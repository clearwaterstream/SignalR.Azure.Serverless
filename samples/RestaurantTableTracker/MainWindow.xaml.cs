using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using SignalR.Azure.Serverless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RestaurantTableTracker
{    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<int, TableStatusControl> tblControlsById = new Dictionary<int, TableStatusControl>();

        SignalRHubHelper signalRHubHelper;

        HttpClient signalRApiHttpClient;
        SignalRApiClient signalRApiClient;

        HubConnection signalRHubConnection;

        public MainWindow()
        {
            InitializeComponent();

            InitTables();
        }

        private void InitTables()
        {
            var latestTableData = new LatestTableData(); // pretend we resolved it from an API, etc

            foreach (var entry in latestTableData)
            {
                var controlName = $"tbl{entry.TableId}";

                var tblControl = new TableStatusControl() { TableId = entry.TableId, Status = entry.Status, Name = controlName, Margin = new Thickness(3) };

                tblControl.StatusChanged += UserChangedTableStatus;

                tblControlsById.Add(entry.TableId, tblControl);

                pnlTables.Children.Add(tblControl);
            }
        }

        async Task InitSignalR()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.AddUserSecrets("a0aa8545-9a28-485e-8218-9851c41dcfbb");

            var config = configBuilder.Build();

            // copy the CONNECTION STRING value from the Azure Portal under SingalR > [item] > Keys (under Settings)
            // then in the csproj dir, run dotnet user-secrets set "signalRConnString" "conn_string_value"
            var connStr = config["signalRConnString"];

            if (string.IsNullOrEmpty(connStr))
                throw new ArgumentNullException(nameof(connStr), "set the CONNECTION STRING value from the Azure Portal under SingalR > [item] > Keys (under Settings)");

            signalRHubHelper = new SignalRHubHelper(connStr);

            signalRApiHttpClient = new HttpClient();
            signalRApiClient = new SignalRApiClient(signalRApiHttpClient, signalRHubHelper);

            var hubUrl = signalRHubHelper.ClientUrl(hubName: "default");

            signalRHubConnection = new HubConnectionBuilder().WithUrl(hubUrl, option =>
            {
                option.AccessTokenProvider = () =>
                {
                    var token = signalRHubHelper.GenerateAccessToken(hubUrl, "user-x");

                    return Task.FromResult(token);
                };
            }).Build();

            signalRHubConnection.On<int, string>("StatusChanged", (tableId, status) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if(tblControlsById.TryGetValue(tableId, out TableStatusControl tblControl))
                    {
                        tblControl.Status = status;

                        lblLastAction.Content = $"Status updated on {DateTime.Now}";
                    }
                });
            });

            await signalRHubConnection.StartAsync();

            lblConnStatus.Content = "Connected";
            lblConnStatus.Background = Brushes.SeaGreen;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await InitSignalR();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            signalRApiHttpClient?.Dispose();

            signalRHubConnection?.DisposeAsync(); // awaiting this causes an issue ...
        }

        async Task UserChangedTableStatus(int tableId, string status)
        {
            var msg = new SignalRMessage()
            {
                target = "StatusChanged",
                arguments = new object[] { tableId, status }
            };

            await signalRApiClient.BroadcastToAllClients(senderUserId: "user-x", hubName: "default", msg);
        }
    }
}
