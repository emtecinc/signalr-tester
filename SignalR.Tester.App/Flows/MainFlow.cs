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

using SignalR.Tester.App.Commands;
using SignalR.Tester.App.Utils;
using SignalR.Tester.Utils.XConsole;
using System;
using System.Drawing;

namespace SignalR.Tester.App.Flows
{
    class MainFlow
    {
        public void Run()
        {
            ConsoleWriter.ShowMaximum();

            ConsoleWriter.WriteLine("Welcome to SignalR Test Tool !!", Color.Yellow);

            ConsoleWriter.WriteLine();

            ConsolePosition.MainMenuLastPosition = new Point(Console.CursorLeft, Console.CursorTop);

            var consoleFlow = new ConsoleWizard
            {
                CursorPositionChanged = OnCursorPositionChanged
            };

            var menuSelectionPage = new ConsoleOptionsPage("What would you like to do ?", ConsoleColor.Cyan);
            menuSelectionPage.AddOption("Start a new load test");
            menuSelectionPage.AddOption("Start sending message on an running load");
            menuSelectionPage.AddOption("Stop the load test");
            menuSelectionPage.AddOption("Stop sending message on an running load");
            consoleFlow.AddPage(menuSelectionPage);

            menuSelectionPage.EnableOption(0);
            menuSelectionPage.DisableOption(1);
            menuSelectionPage.DisableOption(2);
            menuSelectionPage.DisableOption(3);

            MenuSelectedCommand command = null;

            do
            {
                ConsoleWriter.ClearConsole(ConsolePosition.MainMenuLastPosition.Y);

                ConsoleWriter.WriteLine();

                var data = consoleFlow.Run();

                var selecteIndex = data.GetMainMenuSelectedIndex();

                if(selecteIndex == 0)
                    command = new MenuSelectedCommand(menuSelectionPage);

                if (selecteIndex.HasValue)
                {
                    command.Execute(selecteIndex.Value);
                }

            } while (true);
        }

        private void OnCursorPositionChanged(Point point)
        {
            ConsolePosition.MainMenuLastPosition = point;
        }

    }
}
