using Microsoft.AspNetCore.SignalR.Client;
using SignalR.Azure.Serverless;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PubSubConsoleApp
{
    public class PubSubSample : IDisposable
    {
        readonly HttpClient signalRApiHttpClient = new HttpClient();
        readonly SignalRHubHelper signalRHubHelper;
        readonly HubConnection signalRHubConnection;

        public PubSubSample(string connString)
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
        }

        public async Task Run()
        {
            // start listening to published events
            await signalRHubConnection.StartAsync();

            Console.WriteLine("Listening for events...");

            var signalRApiClient = new SignalRApiClient(signalRApiHttpClient, signalRHubHelper);

            var msg = new SignalRMessage()
            {
                target = "StatusChanged",
                arguments = new object[] { "device_1", "offline" }
            };

            // send a test message
            await signalRApiClient.BroadcastToAllClients(senderUserId: "user-x", hubName: "default_hub", msg);
        }

        public async Task Stop()
        {
            await signalRHubConnection.DisposeAsync();
        }

        public void Dispose()
        {
            signalRApiHttpClient?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
