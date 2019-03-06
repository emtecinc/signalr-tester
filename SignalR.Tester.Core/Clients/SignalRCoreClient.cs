//Copyright(c) 2019 Emtec Inc

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using SignalR.Tester.Core.Utils;

namespace SignalR.Tester.Core.Clients
{
    class SignalRCoreClient : IClient
    {
        private HubConnection connection;
        private CancellationToken token;
        public Action OnClosed { get; set; }
        public Action<string> OnMessage { get; set; }
        private ConnectionStatus connectionStatus;
        public Action<IEventMessage> OnLogMessage { get; set; }
        public string clientId;
        private readonly ILogger logger;

        public SignalRCoreClient(string clientId)
        {
            logger = LogManager.GetLogger();
            this.clientId = clientId;
        }

        public ConnectionStatus ConnectionStatus
        {
            get
            {
                if (connection == null)
                {
                    return ConnectionStatus.Disconnected;
                }
                else
                {
                    return connectionStatus;
                }
            }
        }

        public async Task CreateAndStartConnection(ConnectionArgument argument, Tuple<string, Func<string, Task<object[]>>> methodToInvoke = null)
        {
            string url = string.Empty;

            if (argument.Url.EndsWith("/"))
                url = argument.Url + argument.Hub;
            else
                url = $"{argument.Url}/{argument.Hub}";

            connection = new HubConnectionBuilder()
               .WithUrl(url, GetTransportType(argument.Transport))
               .Build();

            token = new CancellationToken();

            connection.Closed += _connection_Closed;

            for (int connectCount = 0; connectCount < 2; connectCount++)
            {
                if (!token.IsCancellationRequested)
                {
                    try
                    {
                        await connection.StartAsync(token);

                        connectionStatus = ConnectionStatus.Connected;

                        break;
                    }
                    catch (Exception ex)
                    {
                        OnLogMessage?.Invoke(new EventMessageInfo("Failed to create connection. Will attempt to reconnect...", MessageType.Error));
                        logger.Error(ex);
                    }

                    await Task.Delay(1000);
                }
            }
        }

        public async Task InvokeMethod(string method, Func<string, Task<object[]>> objectGenerator)
        {
            var data = await objectGenerator.Invoke(clientId);

            await connection.InvokeCoreAsync(method, data);

        }

        private async Task _connection_Closed(Exception arg)
        {
            OnClosed?.Invoke();

            await Task.FromResult(connectionStatus = ConnectionStatus.Disconnected);
        }

        public async Task StopConnection()
        {
            connectionStatus = ConnectionStatus.Disconnected;
            await connection.StopAsync();
        }

        private HttpTransportType GetTransportType(TransportType transport)
        {
            switch (transport)
            {
                case TransportType.WebSockets: return HttpTransportType.WebSockets;
                case TransportType.ServerSentEvents: return HttpTransportType.ServerSentEvents;
                case TransportType.LongPolling: return HttpTransportType.LongPolling;
                case TransportType.Auto: return HttpTransportType.None;
                default: return HttpTransportType.None;
            }
        }
    }
}
