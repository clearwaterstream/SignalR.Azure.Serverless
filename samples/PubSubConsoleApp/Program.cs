using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace PubSubConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.AddUserSecrets("a0aa8545-9a28-485e-8218-9851c41dcfbb");

            var config = configBuilder.Build();

            // copy the CONNECTION STRING value from the Azure Portal under SingalR > [item] > Keys (under Settings)
            // then in the csproj dir, run dotnet user-secrets set "signalRConnString" "conn_string_value"
            var connStr = config["signalRConnString"];

            using (var sample = new PubSubSample(connStr))
            {
                await sample.Run();

                await Task.Delay(3000);

                await sample.Stop();
            }

            Console.Read();
        }
    }
}
