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

using DustInTheWind.ConsoleTools.InputControls;
using System;
using System.Drawing;

namespace SignalR.Tester.Utils.XConsole
{
    public class ConsoleReadPage : IConsoleWizardPage
    {
        private ValueView<string> input;

        public Func<int, object, bool> CanNavigateToNextPage { get; set; }

        public bool IsExitSelected => false;

        public Action<Point> OnCursorPositionChanged { get; set; }

        public ConsoleReadPage(string caption)
        {
            input = new ValueView<string>(caption);
            input.CursorPositionChanged = OnCursorPositionChanged;
        }

        public ConsoleReadPage(string caption, ConsoleColor captionColor) : this(caption)
        {
            input.Label.ForegroundColor = captionColor;
        }

        public object Display()
        {
            return input.Read();
        }

        public void AddExitOption(string caption)
        {
            
        }
    }
}
