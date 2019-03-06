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

using SignalR.Tester.Core;
using SignalR.Tester.Utils.XConsole;
using System;
using System.Collections.Generic;

namespace SignalR.Tester.App.Utils
{
    public static class ResultExtensions
    {
        public static bool TryParseProperty<T>(this object obj, string propertyName, out T result)
        {
            var canBeConverted = obj.GetType().GetProperty(propertyName)?.GetValue(obj) is T;

            if (canBeConverted)
                result = (T)Convert.ChangeType(obj.GetType().GetProperty(propertyName).GetValue(obj), typeof(T));
            else
                result = default(T);

            return canBeConverted;
        }

        public static int? GetMainMenuSelectedIndex(this List<ConsoleFlowResult> data)
        {
            
            if (data[0].Output is ConsoleOptionsPageResult)
            {
                return ((ConsoleOptionsPageResult)data[0].Output).SelectedIndex;
            }
            else
                return null;
        }

        public static ConnectionArgument ConvertToLoadArgument(this List<ConsoleFlowResult> data)
        {
            var loadArgument = new ConnectionArgument();

            var textParameters = data[0].Output as List<string>;

            loadArgument.Url = textParameters[0];
            loadArgument.Hub = textParameters[1];
            loadArgument.Connections = Convert.ToInt32(textParameters[2]);

            if (data[1].Output is ConsoleOptionsPageResult)
            {
                loadArgument.Agent = ((ConsoleOptionsPageResult)data[1].Output).SelectedText;
            }

            if (data[2].Output is ConsoleOptionsPageResult)
            {
                var selectedTransportIndex = ((ConsoleOptionsPageResult)data[2].Output).SelectedIndex;

                switch (selectedTransportIndex)
                {
                    case 0: loadArgument.Transport = TransportType.WebSockets; break;
                    case 1: loadArgument.Transport = TransportType.LongPolling; break;
                    case 2: loadArgument.Transport = TransportType.ServerSentEvents; break;
                    case 3: loadArgument.Transport = TransportType.Auto; break;
                }
            }

            if (data[3].Output is ConsoleOptionsPageResult)
            {
                var selectedClientTypeIndex = ((ConsoleOptionsPageResult)data[3].Output).SelectedIndex;

                switch (selectedClientTypeIndex)
                {
                    case 0: loadArgument.ClientType = ClientType.SignalR; break;
                    case 1: loadArgument.ClientType = ClientType.SignalRCore; break;
                }
            }

            return loadArgument;
        }

        public static MessageSendArgument ConvertToStressArgument(this List<ConsoleFlowResult> data)
        {
            var messageSendArgument = new MessageSendArgument();

            if (data[0].Output is ConsoleOptionsPageResult)
            {
                messageSendArgument.Method = ((ConsoleOptionsPageResult)data[0].Output).SelectedText;
            }

            var textParameters = data[1].Output as List<string>;

            messageSendArgument.TimeBetweenSends = Convert.ToInt32(textParameters[0]);
            messageSendArgument.Timeout = Convert.ToInt32(textParameters[1]);

            return messageSendArgument;
        }
    }
}
