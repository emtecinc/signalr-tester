using System;
using System.Drawing;
using System.Threading;
using Console = Colorful.Console;

namespace SignalR.Tester.Core
{
    public static class CommandExecutor
    {
        public static void Run(IAgent agent)
        {
            string command = string.Empty;

            Console.WriteLineFormatted("Type {0} at the command prompt, and press {1} to abort the ramp up ...", Color.Orange, Color.Wheat, "exit", "enter");

            while (command.ToLower() != "exit")
            {
                command = Console.ReadLine();

                switch (command.ToLower())
                {
                    case "exit":

                        HandleExit(agent);

                        break;

                    default:

                        Console.WriteLine("Oops !! Couldn't understand the command....", Color.Red);
                        Console.WriteLine("Type exit at the command prompt to exit....", Color.Wheat);
                        break;
                }

            }
        }

        
    }
}
