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
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.AspNet.SignalR.Client.Transports;
using SignalR.Tester.Core.Utils;

namespace SignalR.Tester.Core.Clients
{
    public class SignalRClient : IClient
    {
        private Connection connection;
        private CancellationTokenSource token;
        private IHubProxy proxy;
        public Action<IEventMessage> OnLogMessage { get; set; }
        public Action<int, int> OnConnectionStatusChanged { get; set; }
        public Action OnClosed { get; set; }
        public Action<string> OnMessage { get; set; }
        public string clientId;
        private readonly ILogger logger;

        public SignalRClient(string clientId)
        {
            logger = LogManager.GetLogger();
            this.clientId = clientId;
        }

        private void OnClosedInternal()
        {
            OnClosed?.Invoke();
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
                    switch (connection.State)
                    {
                        case ConnectionState.Connected: return ConnectionStatus.Connected;
                        case ConnectionState.Connecting: return ConnectionStatus.Connecting;
                        case ConnectionState.Disconnected: return ConnectionStatus.Disconnected;
                        case ConnectionState.Reconnecting: return ConnectionStatus.Reconnecting;
                        default: return ConnectionStatus.Disconnected;
                    }
                }
            }
        }

        public async Task CreateAndStartConnection(ConnectionArgument argument, Tuple<string, Func<string, Task<object[]>>> methodToInvoke = null)
        {
            var connection = new HubConnection(argument.Url);
            proxy = connection.CreateHubProxy(argument.Hub);
            this.connection = connection;
            token = new CancellationTokenSource();

            this.connection.Closed += OnClosedInternal;

            for (int connectCount = 0; connectCount < 2; connectCount++)
            {
                if (!token.IsCancellationRequested)
                {
                    try
                    {
                        await this.connection.Start(GetTransportType(argument.Transport));

                        if (methodToInvoke != null)
                            await InvokeMethod(methodToInvoke.Item1, methodToInvoke.Item2);

                        break;
                    }
                    catch (Exception ex)
                    {
                        OnLogMessage?.Invoke(new EventMessageInfo("Failed to create connection. Will attempt to reconnect...", MessageType.Error));
                        logger.Error(ex);
                    }

                    await Task.Delay(1000);
                }
                else
                    break;
            }
        }

        public async Task InvokeMethod(string method, Func<string, Task<object[]>> objectGenerator)
        {
            var data = await objectGenerator?.Invoke(clientId);

            await proxy.Invoke(method, data);

        }

        public async Task StopConnection()
        {
            await Task.Run(() =>
            {
                token?.Cancel();
                connection?.Stop();
            });
        }

        private IClientTransport GetTransportType(TransportType transport)
        {
            switch (transport)
            {
                case TransportType.WebSockets: return new WebSocketTransport(); ;
                case TransportType.ServerSentEvents: return new ServerSentEventsTransport();
                case TransportType.LongPolling: return new LongPollingTransport();
                case TransportType.Auto: return new AutoTransport(new DefaultHttpClient());
                default: return new AutoTransport(new DefaultHttpClient());
            }
        }
    }
}
