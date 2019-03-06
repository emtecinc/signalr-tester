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
using System.Drawing;
using System.Threading;
using SignalR.Tester.App.Utils;
using SignalR.Tester.Core;
using SignalR.Tester.Utils.XConsole;

namespace SignalR.Tester.App.Commands
{
    class StopStressTestCommand
    {
        private readonly IAgent agent;

        public StopStressTestCommand(IAgent agent)
        {
            this.agent = agent;
        }

        public void Execute()
        {
            ConsolePosition.MainMenuLastPosition = new Point(Console.CursorLeft,Console.CursorTop);

            ConsoleWriter.WriteLine();
            ConsoleWriter.WriteLine();

            var tokenCancellation = new CancellationTokenSource();

            agent.StopStress(tokenCancellation).Wait();

        }
    }
}
