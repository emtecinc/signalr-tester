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

using SignalR.Tester.App.Mef.Composition;
using SignalR.Tester.Utils.XConsole;
using System;
using System.Collections.Generic;

namespace SignalR.Tester.App.Flows
{
    class LoadTestFlow
    {
        public List<ConsoleFlowResult> Run()
        {
            var consoleFlow = new ConsoleWizard
            {
                IsStepCountVisible = true
            };

            var basicConfigurationPage = new ConsoleReadItemsPage(new List<ConsoleReadItem> {
                new ConsoleReadItem{Question = "What is the target URL of Hub? :", Validator = TargetUrlValidator, ValidationErrorMessage = "Please enter a valid url"},
                new ConsoleReadItem{Question = "what is the name of Hub? :"},
                new ConsoleReadItem{Question = "How many connections you want to ramp up? :", ItemType = typeof(int), ValidationErrorMessage = "Please enter a numeric value"}
            }, ConsoleColor.Cyan);

            var agentSelectionPage = new ConsoleOptionsPage("Which of the below Agents should run the test?", ConsoleColor.Cyan);
            var agentList = AgentBuilder.GetAgentList();
            agentList.ForEach(agent => agentSelectionPage.AddOption(agent));
            agentSelectionPage.AddExitOption();

            var transportSelectionPage = new ConsoleOptionsPage("Choose the transport mechanism?", ConsoleColor.Cyan);
            transportSelectionPage.AddOption("Websockets");
            transportSelectionPage.AddOption("Long Polling");
            transportSelectionPage.AddOption("Server Sent Events");
            transportSelectionPage.AddOption("Auto");

            var clientTypeSelectionPage = new ConsoleOptionsPage("Choose the client type?", ConsoleColor.Cyan);
            clientTypeSelectionPage.AddOption("SignalR");
            clientTypeSelectionPage.AddOption("SignalR Core");

            consoleFlow.AddPage(basicConfigurationPage);
            consoleFlow.AddPage(agentSelectionPage);
            consoleFlow.AddPage(transportSelectionPage);
            consoleFlow.AddPage(clientTypeSelectionPage);

            return consoleFlow.Run();
        }

        private bool TargetUrlValidator(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }
    }
}
