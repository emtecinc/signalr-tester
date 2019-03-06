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

using SignalR.Tester.Core;
using SignalR.Tester.Core.Agents;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;

namespace Demo.SignalR.Agent
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IAgent))]
    [ExportMetadata("AgentName", "SignalR Chat Hub Agent")]
    [ExportMetadata("AgentDescription", "Agent for testing microsoft sample ChatHub")]

    public class ChatHubAgent : AgentBase
    {
        List<string> RandomUsers;

        [ImportingConstructor]
        public ChatHubAgent(Recomposable<ConnectionArgument> arguments) : base(arguments)
        {
            RandomUsers = new List<string>();
        }

        protected override Tuple<string, Func<string, Task<object[]>>> MethodToInvokeOnAgentStarted()
        {
            return new Tuple<string, Func<string, Task<object[]>>>("LogOn", LogOn);
        }

        protected override List<Tuple<string, Func<string, Task<object[]>>>> PreRegisterMethods()
        {
            List<Tuple<string, Func<string, Task<object[]>>>> methods = new List<Tuple<string, Func<string, Task<object[]>>>>
            {
                new Tuple<string, Func<string, Task<object[]>>>("Send", Send)
            };

            return methods;
        }

        private Task<object[]> LogOn(string data)
        {
            var randomUser = Guid.NewGuid().ToString();
            RandomUsers.Add(randomUser);
            return Task.FromResult(new object[] { randomUser });
        }

        private Task<object[]> Send(string data)
        {
            //pick up a random user from list
            Random rnd = new Random();
            var randomPosition = rnd.Next(0, RandomUsers.Count - 1);
            var randomUser = RandomUsers[randomPosition];
            return Task.FromResult(new object[] { randomUser, RandomString(10, true) });
        }

        private string RandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
    }
}
