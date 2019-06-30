using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
        // copy the CONNECTION STRING value from the Azure Portal under SingalR > [item] > Keys (under Settings)
        // then in the csproj dir, run dotnet user-secrets set "signalRConnString" "conn_string_value"
        string singalRConnectionString;

        public MainWindow()
        {
            InitializeComponent();

            InitTables();

            InitSignalR();
        }

        private void InitTables()
        {
            var latestTableData = new LatestTableData(); // pretend we resolved it from an API, etc

            int i = 1;
            foreach (var entry in latestTableData)
            {
                var controlName = $"tbl{i}";

                pnlTables.Children.Add(new TableStatusControl() { TableName = entry.TableName, Status = entry.Status, Name = controlName, Margin = new Thickness(3) });
            }
        }

        void InitSignalR()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.AddUserSecrets("a0aa8545-9a28-485e-8218-9851c41dcfbb");

            var config = configBuilder.Build();

            singalRConnectionString = config["signalRConnString"];

            // dotnet user-secrets set "signalRConnString" "conn_string_value"
            if (string.IsNullOrEmpty(singalRConnectionString))
                throw new ArgumentNullException(nameof(singalRConnectionString), "copy the CONNECTION STRING value from the Azure Portal under SingalR > [item] > Keys (under Settings)");
        }
    }
}
