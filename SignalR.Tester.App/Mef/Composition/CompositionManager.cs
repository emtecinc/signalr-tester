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
using SignalR.Tester.Core.Agents;
using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;

namespace SignalR.Tester.App.Mef.Composition
{
    public static class CompositionManager
    {
        private static readonly Lazy<CompositionContainer> _container = new Lazy<CompositionContainer>(InitContainer);

        private static CompositionContainer InitContainer()
        {
            var pluginPath = ConfigurationManager.AppSettings["AgentPluginPath"]?.ToString();
            if (string.IsNullOrEmpty(pluginPath))
            {
                throw new ConfigurationErrorsException("Plugin Path not found. Please configure AgentPluginPath setting in App.config");
            }

            var directoryCatalog = new DirectoryCatalog(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginPath), "*.dll");
            var typeCatalog = new TypeCatalog(typeof(IAgent), typeof(Recomposable<ConnectionArgument>));

            var aggregateCatalog = new AggregateCatalog(typeCatalog, directoryCatalog);

            return new CompositionContainer(aggregateCatalog);
        }

        public static CompositionContainer Container
        {
            get { return _container.Value; }
        }
    }
}
