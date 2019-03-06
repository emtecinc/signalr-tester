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

using SignalR.Tester.App.Flows;
using SignalR.Tester.Utils.XConsole;
using SignalR.Tester.App.Utils;
using SignalR.Tester.Core;
using SignalR.Tester.App.Entity;
using System;

namespace SignalR.Tester.App.Commands
{
    class MenuSelectedCommand
    {
        private readonly ConsoleOptionsPage optionsPage;
        private IAgent agent = null;

        public MenuSelectedCommand(ConsoleOptionsPage optionsPage)
        {
            this.optionsPage = optionsPage;
        }

        public void Execute(int selectedIndex)
        {
            var operationType = Enum.Parse(typeof(OperationType), selectedIndex.ToString());

            switch (operationType)
            {
                case OperationType.StartLoadTest:

                    var loadTestFlow = new LoadTestFlow();
                    var loadTestFlowResult = loadTestFlow.Run();
                    var loadArgument = loadTestFlowResult.ConvertToLoadArgument();
                    var loadTestCommand = new NewLoadTestCommand();
                    agent = loadTestCommand.Execute(loadArgument);

                    optionsPage.DisableOption(0);
                    optionsPage.EnableOption(1);
                    optionsPage.EnableOption(2);
                    optionsPage.DisableOption(3);

                    break;

                case OperationType.StartStressTest:

                    if (agent != null)
                    {
                        var stressTestFlow = new StressTestFlow(agent.RegisteredMethods);
                        var stressTestFlowResult = stressTestFlow.Run();
                        if (stressTestFlowResult != null)
                        {
                            var messageSendArgument = stressTestFlowResult.ConvertToStressArgument();
                            var stressTestCommand = new NewStressTestCommand(agent);
                            stressTestCommand.Execute(messageSendArgument);

                            optionsPage.DisableOption(0);
                            optionsPage.DisableOption(1);
                            optionsPage.EnableOption(2);
                            optionsPage.EnableOption(3);
                        }
                    }

                    break;

                case OperationType.StopLoadTest:

                    if (agent != null)
                    {
                        var exitCommand = new StopLoadTestCommand(agent);
                        optionsPage.DisableOption(0);
                        optionsPage.DisableOption(1);
                        optionsPage.DisableOption(2);
                        optionsPage.DisableOption(3);

                        exitCommand.Execute();

                        optionsPage.EnableOption(0);
                    }
                    break;

                case OperationType.StopStressTest:

                    if (agent != null)
                    {
                        var exitCommand = new StopStressTestCommand(agent);

                        optionsPage.DisableOption(1);
                        optionsPage.DisableOption(3);

                        exitCommand.Execute();

                        optionsPage.EnableOption(1);
                        optionsPage.DisableOption(3);
                    }
                    break;
            }


        }
    }
}
