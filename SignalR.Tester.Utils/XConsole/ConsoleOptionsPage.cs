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

using DustInTheWind.ConsoleTools.Menues;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SignalR.Tester.Utils.XConsole
{
    public class ConsoleOptionsPage : IConsoleWizardPage
    {
        private TextMenu textMenu;
        private int itemCount = 0;
        private bool IsExitVisible = false;
        public bool IsExitSelected { get; private set; }
        public Action<Point> OnCursorPositionChanged { get; set; }
        public Func<int, object, bool> CanNavigateToNextPage { get; set; }

        private List<ConsoleOptionPageItem> textMenuItems = new List<ConsoleOptionPageItem>();

        public void EnableOption(int index)
        {
            textMenuItems[index].IsEnabled = true;
        }

        public void DisableOption(int index)
        {
            textMenuItems[index].IsEnabled = false;
        }

        public ConsoleOptionsPage(string caption, string question, ConsoleColor captionColor, ConsoleColor captionBackgroundColor) : this(caption, captionColor, captionBackgroundColor)
        {
            textMenu.QuestionText = question;
        }

        public ConsoleOptionsPage(string caption, ConsoleColor captionColor, ConsoleColor captionBackgroundColor) : this(caption, captionColor)
        {
            textMenu.BackgroundColor = captionBackgroundColor;
        }

        public ConsoleOptionsPage(string caption, ConsoleColor captionColor) : this(caption)
        {
            textMenu.ForegroundColor = captionColor;

        }

        public ConsoleOptionsPage(string caption)
        {
            textMenu = new TextMenu
            {
                TitleText = caption,
                CursorPositionChanged = OnCursorPositionChanged
            };
        }

        public ConsoleOptionsPage(string caption, string question) : this(caption)
        {
            textMenu.QuestionText = question;
        }

        public ConsoleOptionsPage(string caption, string question, Func<int, object, bool> canNavigateToNextPage) : this(caption, question)
        {
            CanNavigateToNextPage = canNavigateToNextPage;
        }

        public ConsoleOptionsPage(string caption, Func<int, object, bool> canNavigateToNextPage) : this(caption)
        {
            CanNavigateToNextPage = canNavigateToNextPage;
        }

        public ConsoleOptionsPage(string caption, string question, ConsoleColor captionColor, ConsoleColor captionBackgroundColor, Func<int, object, bool> canNavigateToNextPage) : this(caption, question, captionColor, captionBackgroundColor)
        {
            CanNavigateToNextPage = canNavigateToNextPage;
        }

        public void AddOption(string caption)
        {
            itemCount++;

            ConsoleOptionPageItem item = new ConsoleOptionPageItem(caption, itemCount.ToString());

            var textMenuItem = item.GetInstance() as TextMenuItem;

            textMenuItems.Add(item);

            textMenu.AddItem(textMenuItem);
        }

        public void AddExitOption(string caption = "Exit")
        {
            if (!IsExitVisible)
            {
                textMenu.AddItem(new TextMenuItem
                {
                    Id = (++itemCount).ToString(),
                    Text = caption
                });

                IsExitVisible = true;
            }
        }

        public object Display()
        {
            textMenu.CursorPositionChanged = OnCursorPositionChanged;

            textMenu.Display();

            if (IsExitVisible && textMenu.SelectedItem.Id == itemCount.ToString())
                IsExitSelected = true;

            return new ConsoleOptionsPageResult { SelectedIndex = textMenu.SelectedIndex.Value, SelectedText = textMenu.SelectedItem.Text };
        }

    }

    public class ConsoleOptionsPageResult
    {
        public int SelectedIndex { get; set; }
        public string SelectedText { get; set; }
    }
}
