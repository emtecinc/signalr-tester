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
using System.Runtime.InteropServices;

namespace SignalR.Tester.Utils.XConsole
{
    public class ConsoleWriter
    {
        public static void WriteLine()
        {
            Colorful.Console.WriteLine();
        }

        public static void WriteLine(Point writePosition)
        {
            WriteWithPosition(writePosition, () => Colorful.Console.WriteLine());
        }

        public static void WriteLine(Point writePosition, Point resetPosition)
        {
            WriteWithPosition(writePosition, resetPosition, () => Colorful.Console.WriteLine());
        }

        public static void WriteLine(string text)
        {
            Colorful.Console.WriteLine(text);
        }

        public static void WriteLine(string text, Point writePosition)
        {
            WriteWithPosition(writePosition, () => Colorful.Console.WriteLine(text));
        }

        public static void WriteLine(string text, Point writePosition, Point resetPosition)
        {
            WriteWithPosition(writePosition, resetPosition, () => Colorful.Console.WriteLine(text));
        }

        public static void WriteLine(string text, Color color)
        {
            Colorful.Console.WriteLine(text, color);
        }

        public static void WriteLine(string text, Color color, Point writePosition)
        {
            WriteWithPosition(writePosition, () => Colorful.Console.WriteLine(text, color));
        }

        public static void WriteLine(string text, Color color, Point writePosition, Point resetPosition)
        {
            WriteWithPosition(writePosition, resetPosition, () => Colorful.Console.WriteLine(text, color));
        }

        public static void WriteLineFormatted(string text, Color baseColor, Color argumentColor, params object[] arguments)
        {
            Colorful.Console.WriteLineFormatted(text, argumentColor, baseColor, arguments);
        }

        public static void WriteLineFormatted(string text, Color baseColor, Color argumentColor, Point writePosition, params object[] arguments)
        {
            WriteWithPosition(writePosition, () => Colorful.Console.WriteLineFormatted(text, argumentColor, baseColor, arguments));
        }

        public static void WriteLineFormatted(string text, Color baseColor, Color argumentColor, Point writePosition, Point resetPosition, params object[] arguments)
        {
            WriteWithPosition(writePosition, resetPosition, () => Colorful.Console.WriteLineFormatted(text, argumentColor, baseColor, arguments));
        }

        public static void WriteLineFormatted(string text, Color baseColor, params object[] arguments)
        {
            Colorful.Console.WriteLineFormatted(text, baseColor, baseColor, arguments);
        }

        public static void WriteLineFormatted(string text, Color baseColor, Point writePosition, params object[] arguments)
        {
            WriteWithPosition(writePosition, () => Colorful.Console.WriteLineFormatted(text, baseColor, baseColor, arguments));
        }

        public static void WriteLineFormatted(string text, Color baseColor, Point writePosition, Point resetPosition, params object[] arguments)
        {
            WriteWithPosition(writePosition, resetPosition, () => Colorful.Console.WriteLineFormatted(text, baseColor, baseColor, arguments));
        }

        public static void Write(string text)
        {
            Colorful.Console.Write(text);
        }

        public static void Write(string text, Point writePosition)
        {
            WriteWithPosition(writePosition, () => Colorful.Console.Write(text));
        }

        public static void Write(string text, Point writePosition, Point resetPosition)
        {
            WriteWithPosition(writePosition, resetPosition, () => Colorful.Console.Write(text));
        }

        public static void Write(string text, Color color)
        {
            Colorful.Console.Write(text, color);
        }

        public static void Write(string text, Color color, Point writePosition)
        {
            WriteWithPosition(writePosition, () => Colorful.Console.Write(text, color));
        }

        public static void Write(string text, Color color, Point writePosition, Point resetPosition)
        {
            WriteWithPosition(writePosition, resetPosition, () => Colorful.Console.Write(text, color));
        }

        public static void WriteFormatted(string text, Color baseColor, Color argumentColor, params object[] arguments)
        {
            Colorful.Console.WriteFormatted(text, argumentColor, baseColor, arguments);
        }

        public static void WriteFormatted(string text, Color baseColor, Color argumentColor, Point writePosition, params object[] arguments)
        {
            WriteWithPosition(writePosition, () => Colorful.Console.WriteFormatted(text, argumentColor, baseColor, arguments));
        }

        public static void WriteFormatted(string text, Color baseColor, Color argumentColor, Point writePosition, Point resetPosition, params object[] arguments)
        {
            WriteWithPosition(writePosition, resetPosition, () => Colorful.Console.WriteFormatted(text, argumentColor, baseColor, arguments));
        }

        public static void WriteFormatted(string text, Color baseColor, params object[] arguments)
        {
            Colorful.Console.WriteFormatted(text, baseColor, baseColor, arguments);
        }

        public static void WriteFormatted(string text, Color baseColor, Point writePosition, params object[] arguments)
        {
            WriteWithPosition(writePosition, () => Colorful.Console.WriteFormatted(text, baseColor, baseColor, arguments));
        }

        public static void WriteFormatted(string text, Color baseColor, Point writePosition, Point resetPosition, params object[] arguments)
        {
            WriteWithPosition(writePosition, resetPosition, () => Colorful.Console.WriteFormatted(text, baseColor, baseColor, arguments));
        }

        private static void WriteWithPosition(Point position, Action action)
        {
            var oldPosition = new Point(Console.CursorLeft, Console.CursorTop);
            Console.SetCursorPosition(0, position.Y);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(position.X, position.Y);
            action();
            Console.SetCursorPosition(oldPosition.X, oldPosition.Y);
        }

        private static void WriteWithPosition(Point position, Point resetPosition, Action action)
        {
            Console.SetCursorPosition(0, position.Y);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(position.X, position.Y);
            action();
            Console.SetCursorPosition(resetPosition.X, resetPosition.Y);
        }

        public static void ClearConsole(int position)
        {
            for (int startPosition = position + 1; startPosition <= 30; startPosition++)
            {
                Console.SetCursorPosition(0, startPosition);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, position);
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int HIDE = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;
        public static void ShowMaximum()
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(ThisConsole, MAXIMIZE);
        }

    }
}
