using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SignalR.Azure.Serverless.Test
{
    [Collection("Sequential")]
    public class ApiClientTest : IClassFixture<ConfigFixture>, IDisposable
    {
        private readonly ConfigFixture configFixture;
        private readonly ITestOutputHelper output;

        private readonly HttpClient signalRApiHttpClient = new HttpClient();

        public ApiClientTest(ConfigFixture configFixture, ITestOutputHelper output)
        {
            this.configFixture = configFixture;
            this.output = output;
        }

        [Fact]
        public async Task BroadcastToAllClients()
        {
            var signalRHelper = new SignalRHubHelper(configFixture.SignalRConnString);
            var signalRApiClient = new SignalRApiClient(signalRApiHttpClient, signalRHelper);

            var msg = new SignalRMessage()
            {
                target = "StatusChanged",
                arguments = new object[] { "device_1", "offline" }
            };

            await signalRApiClient.BroadcastToAllClients("usex-x", "default_hub", msg);

            output.WriteLine("msg sent");
        }

        public void Dispose()
        {
            signalRApiHttpClient?.Dispose();
        }
    }
}
