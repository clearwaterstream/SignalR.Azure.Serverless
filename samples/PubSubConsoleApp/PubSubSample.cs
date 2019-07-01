using Microsoft.AspNetCore.SignalR.Client;
using SignalR.Azure.Serverless;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PubSubConsoleApp
{
    public class PubSubSample
    {
        SignalRHubHelper signalRHubHelper;
        HubConnection signalRHubConnection;

        public async Task ListenToEvents(string connString)
        {
            signalRHubHelper = new SignalRHubHelper(connString);

            var hubUrl = signalRHubHelper.ClientUrl("default_hub");

            // define a connection to receive published events
            signalRHubConnection = new HubConnectionBuilder().WithUrl(hubUrl, option =>
            {
                option.AccessTokenProvider = () =>
                {
                    var token = signalRHubHelper.GenerateAccessToken(hubUrl, "user-x");

                    return Task.FromResult(token);
                };
            }).Build();

            // define an event handler for received messages
            signalRHubConnection.On<string, string>("StatusChanged", (deviceId, status) =>
            {
                Console.WriteLine($"Device {deviceId} changed status to {status}");
            });

            // start listening to published events
            await signalRHubConnection.StartAsync();

            Console.WriteLine("Listening for events...");
        }

        public async Task PublishTestMessage()
        {
            using (var httpClient = new HttpClient()) // it is recommened that you re-use the HttpClient
            {
                var signalRApiClient = new SignalRApiClient(httpClient, signalRHubHelper);

                var msg = new SignalRMessage()
                {
                    target = "StatusChanged",
                    arguments = new object[] { "device_1", "offline" }
                };

                await signalRApiClient.BroadcastToAllClients(senderUserId: "user-x", hubName: "default_hub", msg);
            }
        }

        public async Task StopListeningToEvents()
        {
            await signalRHubConnection.DisposeAsync();
        }
    }
}
