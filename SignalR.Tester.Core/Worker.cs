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

using SignalR.Tester.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.Tester.Core
{
    public class Worker : IWorker
    {
        private readonly ConcurrentBag<IClient> _clients;
        private WorkerStatusUpdater statusUpdater;
        ConnectionArgument arguments;
        private readonly CancellationTokenSource cancellationTokenForRun;
        private readonly CancellationTokenSource cancellationTokenForStress;
        CountdownEvent countdownEventForRun;
        CountdownEvent countdownEventForStress;
        public Action<IEventMessage> OnLogMessage { get; set; }
        public Action<int, int> OnConnectionStatusChanged { get; set; }
        private readonly ILogger logger;

        public Worker(ConnectionArgument arguments)
        {
            logger = LogManager.GetLogger();
            cancellationTokenForRun = new CancellationTokenSource();
            cancellationTokenForStress = new CancellationTokenSource();
            this.arguments = arguments;
            countdownEventForRun = new CountdownEvent(1);
            countdownEventForStress = new CountdownEvent(1);
            _clients = new ConcurrentBag<IClient>();
        }

        public async Task Run(Tuple<string, Func<string, Task<object[]>>> methodToInvoke = null)
        {
            try
            {
                OnLogMessage?.Invoke(new EventMessageInfo($"Ramping up {arguments.Connections} connections to target address {arguments.Url}", MessageType.Info));

                statusUpdater = new WorkerStatusUpdater(countdownEventForRun, _clients, arguments.Connections)
                {
                    OnLogMessage = OnLogMessageReceived,
                    OnConnectionStatusChanged = OnConnectionStatusChangedReceived
                };

                statusUpdater.RunPolling();

                for (int count = 0; count < arguments.Connections; count++)
                {
                    if (!cancellationTokenForRun.IsCancellationRequested)
                    {
                        try
                        {
                            var clientId = (count + 1).ToString();
                            IClient client = ClientFactory.GetClient(arguments.ClientType, clientId);
                            client.OnLogMessage += OnLogMessageReceived;

                            await client.CreateAndStartConnection(arguments, methodToInvoke);

                            _clients.Add(client);
                        }
                        catch (Exception ex)
                        {
                            OnLogMessage?.Invoke(new EventMessageInfo("Failed to start the connnection for a client", MessageType.Error));
                            logger.Error(ex);
                        }
                    }
                    else
                    {
                        OnLogMessage?.Invoke(new EventMessageInfo($"Worker recieved cancellation request. Aborting ramp up ...", MessageType.Info));
                        countdownEventForRun.Signal();
                        break;
                    }
                }

                if (!cancellationTokenForRun.IsCancellationRequested)
                {
                    OnLogMessage?.Invoke(new EventMessageInfo("Ramp up competed.....", MessageType.Info));
                    countdownEventForRun.Signal();
                }
            }
            catch (Exception ex)
            {
                OnLogMessage?.Invoke(new EventMessageInfo("Failed to ramp up connections", MessageType.Error));
                logger.Error(ex);
                throw;
            }
        }

        public Task Stop(CancellationTokenSource cancellationTokenAtSource)
        {
            return Task.Run(async () =>
            {
                try
                {
                    cancellationTokenForRun.Cancel();

                    countdownEventForRun.Wait(TimeSpan.FromMinutes(1));

                    OnLogMessage?.Invoke(new EventMessageInfo("Worker is stopping connections ...", MessageType.Info));

                    if (!_clients.IsEmpty)
                    {
                        foreach (var client in _clients)
                        {
                            await Task.Run(() => client.StopConnection());
                        }
                    }

                    statusUpdater.RunOnce();

                    cancellationTokenAtSource.Cancel();

                    OnLogMessage?.Invoke(new EventMessageInfo("Connections stopped succesfully", MessageType.Info));
                }
                catch (Exception ex)
                {
                    OnLogMessage?.Invoke(new EventMessageInfo("Failed to stop the connections", MessageType.Error));
                    logger.Error(ex);
                    throw;
                }
            });
        }

        private void OnLogMessageReceived(IEventMessage message)
        {
            OnLogMessage?.Invoke(message);
        }

        private void OnConnectionStatusChangedReceived(int totalClients, int noOfConnectedClients)
        {
            OnConnectionStatusChanged?.Invoke(totalClients, noOfConnectedClients);
        }

        public Task RunStress(string method, int sendIntervalInMilliSeconds, int timeOutInMilliSeconds, Func<string, Task<object[]>> objectGenerator)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentException("Method cannot be empty");
            else
            {
                return Task.Run(async () =>
                {
                    while (!cancellationTokenForStress.IsCancellationRequested)
                    {
                        if (!_clients.IsEmpty)
                        {
                            foreach (var client in _clients)
                            {
                                if (!cancellationTokenForStress.IsCancellationRequested)
                                {
                                    await client.InvokeMethod(method, objectGenerator);
                                    Task.Delay(TimeSpan.FromMilliseconds(sendIntervalInMilliSeconds)).Wait();
                                }
                            }
                        }
                    }

                    countdownEventForStress.Signal();

                });
            }
        }

        public Task StopStress(CancellationTokenSource cancellationTokenAtSource)
        {
            return Task.Run(() =>
            {
                try
                {
                    cancellationTokenForStress.Cancel();

                    countdownEventForStress.Wait(TimeSpan.FromMinutes(1));

                    OnLogMessage?.Invoke(new EventMessageInfo("Worker is stopping stress test ...", MessageType.Info));

                    statusUpdater.RunOnce();

                    cancellationTokenAtSource.Cancel();

                    OnLogMessage?.Invoke(new EventMessageInfo("Stress test stopped succesfully", MessageType.Info));
                }
                catch (Exception ex)
                {
                    OnLogMessage?.Invoke(new EventMessageInfo("Failed to start stress test", MessageType.Error));
                    logger.Error(ex);
                    throw;
                }
            });
        }
    }
}

