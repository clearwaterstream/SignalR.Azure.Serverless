/* Igor Krupin */
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.Azure.Serverless
{
    /// <summary>
    /// Used to interface with SignalR for things like broadcasting messages, etc
    /// See https://github.com/Azure/azure-signalr/blob/dev/docs/swagger/v1.json
    /// </summary>
    public class SignalRApiClient
    {
        readonly HttpClient _httpClient = null;
        readonly SignalRHubHelper _hubHelper;

        public SignalRApiClient(HttpClient httpClient, SignalRHubHelper hubHelper)
        {
            _httpClient = httpClient;

            _hubHelper = hubHelper;

            Endpoint = hubHelper.Endpoint;
        }

        public string Endpoint { get; }

        public static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include
        };

        public async Task<HttpStatusCode> BroadcastToAllClients(string senderUserId, string hubName, SignalRMessage message, CancellationToken cancellationToken = default)
        {
            var pubUrl = BroadcastToAllClientsUrl(hubName);

            var token = _hubHelper.GenerateAccessToken(pubUrl, senderUserId);

            using (var ms = new MemoryStream())
            {
                using(var sw = new StreamWriter(ms))
                {
                    using (var jw = new JsonTextWriter(sw))
                    {
                        var ser = JsonSerializer.Create(_serializerSettings);

                        ser.Serialize(jw, message);

                        jw.Flush();
                        ms.Position = 0;

                        using (var req = new HttpRequestMessage(HttpMethod.Post, pubUrl))
                        {
                            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            using (var reqContent = new StreamContent(ms))
                            {
                                reqContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                                req.Content = reqContent;

                                using (var resp = await _httpClient.SendAsync(req))
                                {
                                    return resp.StatusCode;
                                }
                            }
                        }
                    }
                }
            }
        }

        public string BroadcastToAllClientsUrl(string hubName) => $"{Endpoint}/api/v1/hubs/{hubName.ToLowerInvariant()}";
    }
}
