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

using Konsole;
using SignalR.Tester.App.Mef.Composition;
using SignalR.Tester.App.Utils;
using SignalR.Tester.Core;
using SignalR.Tester.Utils.XConsole;
using System;
using System.Drawing;

namespace SignalR.Tester.App.Commands
{
    class NewLoadTestCommand
    {
        private ProgressBar pbNumberOfCurrentConnectedClients;
        private Point WritableConsolePosition { get; set; }

        public IAgent Execute(ConnectionArgument argument)
        {
            var agent = AgentBuilder.GetAgent(argument);
            agent.OnLogMessage = OnLogMessageReceived;
            agent.OnConnectionStatusChanged = OnConnectionStatusChangedReceived;
            InitProgressBar(argument.Connections);
            agent.StartAgent();
            return agent;
        }

        private void OnLogMessageReceived(IEventMessage message)
        {
            switch (message.Type)
            {
                case MessageType.Info:
                    ConsoleWriter.Write(message.Message, Color.Green, WritableConsolePosition, ConsolePosition.MainMenuLastPosition); break;
                case MessageType.Warning:
                    ConsoleWriter.Write(message.Message, Color.Yellow, WritableConsolePosition, ConsolePosition.MainMenuLastPosition); break;
                case MessageType.Error:
                    ConsoleWriter.Write(message.Message, Color.Red, WritableConsolePosition, ConsolePosition.MainMenuLastPosition); break;
            }
        }

        private void OnConnectionStatusChangedReceived(int totalClients, int noOfConnectedClients)
        {
            pbNumberOfCurrentConnectedClients.Refresh(noOfConnectedClients, "No of curent connected clients");
        }

        private void InitProgressBar(int connections)
        {
            ConsoleWriter.WriteLine();
            ConsoleWriter.WriteLineFormatted("--------------------------------- {0} ---------------------------------", Color.DarkCyan, Color.Yellow, "CONNECTION STATUS");
            pbNumberOfCurrentConnectedClients = new ProgressBar(PbStyle.DoubleLine, connections);
            ConsoleWriter.WriteLine("-------------------------------------------------------------------------------------", Color.DarkCyan);
            ConsoleWriter.WriteLine();

            WritableConsolePosition = new Point(Console.CursorLeft, Console.CursorTop);

            ConsoleWriter.WriteLine();

            ConsolePosition.MainMenuLastPosition = new Point(Console.CursorLeft, Console.CursorTop);
        }

    }
}
