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

using DustInTheWind.ConsoleTools;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SignalR.Tester.Utils.XConsole
{
    public class ConsoleWizard
    {
        private List<IConsoleWizardPage> menus;
        public ConsoleFlowStatus Status { get; private set; }
        public string ExitCaption { get; set; }

        public bool IsStepCountVisible { get; set; } = false;

        public Action<Point> CursorPositionChanged { get; set; }

        public IConsoleWizardPage this[int i]
        {
            get
            {
                return menus[i];
            }
        }

        public ConsoleWizard()
        {
            menus = new List<IConsoleWizardPage>();
            AddExitToLastPage = false;
            ExitCaption = null;
        }

        public bool AddExitToLastPage { get; set; }

        public void AddPages(IEnumerable<IConsoleWizardPage> menu)
        {
            menus.AddRange(menu);
        }

        public void AddPage(IConsoleWizardPage menu)
        {
            menus.Add(menu);
        }

        public List<ConsoleFlowResult> Run()
        {
            Status = ConsoleFlowStatus.Started;

            var oldPostition = new Point(Console.CursorLeft, Console.CursorTop);

            if (menus == null)
                return null;

            if (AddExitToLastPage)
                menus[menus.Count - 1].AddExitOption(ExitCaption);

            var output = new List<ConsoleFlowResult>();

            var pageIndex = 0;

            foreach (var page in menus)
            {
                page.OnCursorPositionChanged = CursorPositionChanged;

                Status = ConsoleFlowStatus.Running;

                if (IsStepCountVisible)
                {
                    CustomConsole.WriteLine(ConsoleColor.Green, $"Step {pageIndex + 1} / {menus.Count}");
                    CustomConsole.WriteLine();
                }

                object result = page.Display();

                if (page.IsExitSelected)
                {
                    ClearConsoleAndRestoreToOldPosition(oldPostition);
                    break;
                }

                if (page.CanNavigateToNextPage != null)
                {
                    bool canNavigateToNextPage = page.CanNavigateToNextPage(pageIndex, result);

                    if (!canNavigateToNextPage)
                    {
                        Status = ConsoleFlowStatus.Aborted;

                        ClearConsoleAndRestoreToOldPosition(oldPostition);

                        break;
                    }
                }

                output.Add(new ConsoleFlowResult { Output = result, SelectedPageIndex = pageIndex });

                ClearConsoleAndRestoreToOldPosition(oldPostition);

                pageIndex++;
            }

            if (Status == ConsoleFlowStatus.Running)
            {
                Status = ConsoleFlowStatus.Completed;
            }

            return output;
        }

        private void ClearConsoleAndRestoreToOldPosition(Point oldPostition)
        {
            ClearConsole(oldPostition.Y, Console.CursorTop);

            Console.SetCursorPosition(oldPostition.X, oldPostition.Y);
        }

        private void ClearConsole(int startTop, int endTop)
        {
            int currentLineCursor = Console.CursorTop;
            for (int start = startTop; start <= endTop; start++)
            {
                Console.SetCursorPosition(0, start);
                Console.Write(new string(' ', Console.WindowWidth));
            }
        }
    }
}
