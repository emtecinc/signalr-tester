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

using SignalR.Tester.Utils.XConsole;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SignalR.Tester.App.Flows
{
    class StressTestFlow
    {
        private readonly List<string> registeredMethodsInAgent;

        public StressTestFlow(List<string> registeredMethodsInAgent)
        {
            this.registeredMethodsInAgent = registeredMethodsInAgent;
        }

        public List<ConsoleFlowResult> Run()
        {
            if (!registeredMethodsInAgent.Any())
            {
                ConsoleWriter.WriteLine("Couldn't find any registered methods in the agent, which can be invoked", Color.Yellow);
                return null;
            }

            var consoleFlow = new ConsoleWizard
            {
                IsStepCountVisible = true
            };

            var agentSelectionPage = new ConsoleOptionsPage("Which method you want the message to be sent to ?", ConsoleColor.Cyan);
            registeredMethodsInAgent.ForEach(method => { agentSelectionPage.AddOption(method); });

            var basicConfigurationPage = new ConsoleReadItemsPage(new List<ConsoleReadItem> {
                new ConsoleReadItem{Question = "Time in milliseconds between sends? :", ItemType = typeof(int), ValidationErrorMessage = "Please enter a numeric value"},
                new ConsoleReadItem{Question = "Timeout in milliseconds? :",ItemType = typeof(int), ValidationErrorMessage = "Please enter a numeric value"}
            }, ConsoleColor.Cyan);

            consoleFlow.AddPage(agentSelectionPage);
            consoleFlow.AddPage(basicConfigurationPage);
  
            return consoleFlow.Run();
        }
    }
}
