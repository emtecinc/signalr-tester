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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.Tester.Core.Agents
{
    public abstract class AgentBase : IAgent
    {
        private readonly ConcurrentDictionary<string, Func<string, Task<object[]>>> RegisteredMethodsInternal;
        private IWorker worker;
        public delegate void OnBeforeAgentStartedHaneler(AgentArgs e);
        public delegate void OnAfterAgentStoppedHanler();

        public List<string> RegisteredMethods => RegisteredMethodsInternal?.Select(val => val.Key).ToList();

        public Action<IEventMessage> OnLogMessage { get; set; }

        public Action<int, int> OnConnectionStatusChanged { get; set; }

        protected AgentBase(Recomposable<ConnectionArgument>  arguments)
        {
            worker = new Worker(arguments.Value)
            {
                OnLogMessage = OnLogMessageReceived,
                OnConnectionStatusChanged = OnConnectionStatusChangedReceived
            };

            RegisteredMethodsInternal = new ConcurrentDictionary<string, Func<string, Task<object[]>>>();

            var methods = PreRegisterMethods();
            if (methods != null)
            {
                methods.ForEach(m => RegisteredMethodsInternal.TryAdd(m.Item1, m.Item2));
            }
        }

        private void OnLogMessageReceived(IEventMessage message)
        {
            OnLogMessage?.Invoke(message);
        }

        private void OnConnectionStatusChangedReceived(int totalClients, int noOfConnectedClients)
        {
            OnConnectionStatusChanged?.Invoke(totalClients, noOfConnectedClients);
        }

        public Task RegisterMethod(string method, Func<string, Task<object[]>> methodToInvoke)
        {
            return Task.FromResult(RegisteredMethodsInternal.TryAdd(method, methodToInvoke));
        }

        public event OnBeforeAgentStartedHaneler OnBeforeAgentStarted;
        public event OnAfterAgentStoppedHanler OnAfterAgentStopped;

        protected abstract List<Tuple<string, Func<string, Task<object[]>>>> PreRegisterMethods();

        protected abstract Tuple<string, Func<string, Task<object[]>>> MethodToInvokeOnAgentStarted();

        public Task StartAgent()
        {
            AgentArgs e = new AgentArgs();
            OnBeforeAgentStarted?.Invoke(e);
            if (!e.Cancel)
            {
                var methodToInvoke = MethodToInvokeOnAgentStarted();
                return worker.Run(methodToInvoke);
            }
            else
                return Task.FromResult("Agent stopped");
        }

        public Task StopAgent(CancellationTokenSource token)
        {
            return Task.Run(async () =>
            {
                await worker.Stop(token);
                OnAfterAgentStopped?.Invoke();
            });
        }

        public Task StartStress(MessageSendArgument argument)
        {
            if (!RegisteredMethodsInternal.ContainsKey(argument.Method))
                throw new ArgumentException("Please register a method first");

            var function = RegisteredMethodsInternal[argument.Method];

            return worker.RunStress(argument.Method, argument.TimeBetweenSends, argument.Timeout, function);
        }

        public Task StopStress(CancellationTokenSource token)
        {
            return worker.StopStress(token);
        }
    }
}
