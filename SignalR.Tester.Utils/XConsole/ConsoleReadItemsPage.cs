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
using System.Collections.Generic;
using System.Drawing;

namespace SignalR.Tester.Utils.XConsole
{
    public class ConsoleReadItemsPage : IConsoleWizardPage
    {
        private List<ConsoleReadItem> questions;
        private ConsoleColor captionColor;

        public ConsoleReadItemsPage(List<ConsoleReadItem> questions)
        {
            this.questions = questions;
        }

        public ConsoleReadItemsPage(List<ConsoleReadItem> questions, ConsoleColor captionColor) : this(questions)
        {
            this.captionColor = captionColor;
        }

        public Func<int, object, bool> CanNavigateToNextPage { get; set; }

        public bool IsExitSelected => false;

        public Action<Point> OnCursorPositionChanged { get; set; }

        public void AddExitOption(string caption)
        {
        }

        public object Display()
        {
            List<string> values = new List<string>();

            questions.ForEach(input =>
            {

                Type type = input.ItemType;

                if (string.IsNullOrEmpty(input.ValidationErrorMessage))
                    input.ValidationErrorMessage = "Input is not in correct format";

                switch (type.Name)
                {
                    case "Int32":
                        var valueViewInt = new ValueView<int>(input.Question);
                        valueViewInt.CursorPositionChanged = OnCursorPositionChanged;

                        valueViewInt.Label.ForegroundColor = captionColor;

                        valueViewInt.TypeConversionErrorMessage = input.ValidationErrorMessage;

                        valueViewInt.CustomParser = delegate (string inputValue)
                        {
                            if (int.TryParse(inputValue, out int result))
                            {
                                if (input.Validator != null)
                                {
                                    if (!input.Validator(inputValue))
                                    {
                                        if (!string.IsNullOrEmpty(input.ValidationErrorMessage))
                                            throw new Exception(input.ValidationErrorMessage);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception(input.ValidationErrorMessage);
                            }

                            return result;
                        };

                        values.Add(valueViewInt.Read().ToString()); break;

                    case "String":
                        var valueViewString = new ValueView<string>(input.Question);
                        valueViewString.CursorPositionChanged = OnCursorPositionChanged;

                        valueViewString.Label.ForegroundColor = captionColor;

                        valueViewString.TypeConversionErrorMessage = input.ValidationErrorMessage;

                        valueViewString.CustomParser = delegate (string inputValue)
                        {
                            if (!string.IsNullOrEmpty(inputValue))
                            {
                                if (input.Validator != null)
                                {
                                    if (!input.Validator(inputValue))
                                    {
                                        if (!string.IsNullOrEmpty(input.ValidationErrorMessage))
                                            throw new Exception(input.ValidationErrorMessage);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception(input.ValidationErrorMessage);
                            }

                            return inputValue;
                        };

                        values.Add(valueViewString.Read().ToString()); break;
                }

            });

            return values;
        }
    }

    public class ConsoleReadItem
    {
        public string Question { get; set; }
        public Type ItemType { get; set; } = typeof(string);
        public string ValidationErrorMessage { get; set; }
        public Func<string, bool> Validator { get; set; }
    }
}
