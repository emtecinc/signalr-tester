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
using SignalR.Tester.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace SignalR.Tester.App.Mef.Composition
{
    public class AgentParts
    {
        private readonly ConnectionArgument argument;
        private static ComposablePart part;

        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<Lazy<IAgent, IAgentMetadata>> Agents;

        public AgentParts(ConnectionArgument argument)
        {
            this.argument = argument;
        }

        public void Compose()
        {
            try
            {
                var batch = new CompositionBatch();

                if (part != null)
                    batch.RemovePart(part);

                part = batch.AddPart(argument);

                CompositionManager.Container.Compose(batch);
                CompositionManager.Container.ComposeParts(this);

            }
            catch (Exception ex)
            {
                LogManager.GetLogger().Error(ex);
                throw;
            }
        }
    }
}
