﻿//Copyright(c) 2019 Emtec Inc

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

using SignalR.Tester.Core;
using SignalR.Tester.Core.Agents;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Demo.SignalR.Core.Agent
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IAgent))]
    [ExportMetadata("AgentName", "SignalR Core Chat Hub Agent")]
    [ExportMetadata("AgentDescription", "Agent for testing microsoft sample ChatHub")]

    public class ChatHubAgent : AgentBase
    {
        [ImportingConstructor]
        public ChatHubAgent(Recomposable<ConnectionArgument> arguments) : base(arguments)
        {
        }

        protected override Tuple<string, Func<string, Task<object[]>>> MethodToInvokeOnAgentStarted()
        {
            return new Tuple<string, Func<string, Task<object[]>>>("Send", Send);
        }

        protected override List<Tuple<string, Func<string, Task<object[]>>>> PreRegisterMethods()
        {
            return null;
        }

        private Task<object[]> Send(string data)
        {
            return Task.FromResult(new object[] { "dummy-user-name","loadtest" });
        }
    }
}
