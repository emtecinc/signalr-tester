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

using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Demo.SignalR.Core.TestHub
{
    public class ChatHub : Hub
    {
        private readonly object _lockAtom = new object();

        public async Task Send(string name, string message)
        {
            await Clients.All.SendAsync("BroadcastMessage", name, message);
        }

        public override Task OnConnectedAsync()
        {
            lock (_lockAtom)
                PerformanceCounter.WebsocketCount++;
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            lock (_lockAtom)
            {
                if (PerformanceCounter.WebsocketCount > 0)
                    PerformanceCounter.WebsocketCount--;
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}