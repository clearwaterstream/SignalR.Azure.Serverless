# SignalR.Azure.Serverless
A helper library for publishing and subscribing to SignalR service running on Azure in serverless mode

With SignalR serverless, you can publish and subscribe to messages without the need to create an ASP.NET Core API App. You may use this library in ASP.NET Core applications, Xamarin, WinForm, WPF, or Console apps. Unlike SNS or Azure Event Grid, any app can subscribe to the feed (and publish) so long as the app knows the `AccessKey`.

SignalR is not a queue, it's an opportunistic broadcaster. SignalR will broadcast an event to whichever clients are connected to the hub at the time of the event. Any clients which are not connected will miss the event. Sometimes this behaviour is desired.

There are two samples: A WFP App [RestaurantTableTracker](https://github.com/clearwaterstream/SignalR.Azure.Serverless/tree/master/samples/RestaurantTableTracker) and a [PubSubConsoleApp](https://github.com/clearwaterstream/SignalR.Azure.Serverless/tree/master/samples/PubSubConsoleApp). See the WPF App for an example on how to handle connection drops.

### Sample
```csharp
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
```
