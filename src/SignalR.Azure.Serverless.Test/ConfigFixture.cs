using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalR.Azure.Serverless.Test
{
    public class ConfigFixture
    {
        public IConfigurationRoot Configuration { get; }
        public string SignalRConnString { get; }

        public ConfigFixture()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.AddUserSecrets("a0aa8545-9a28-485e-8218-9851c41dcfbb");

            Configuration = configBuilder.Build();

            // copy the CONNECTION STRING value from the Azure Portal under SingalR > [item] > Keys (under Settings)
            // then in the csproj dir, run dotnet user-secrets set "signalRConnString" "conn_string_value"
            SignalRConnString = Configuration["signalRConnString"];
        }
    }
}
