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
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;

namespace SignalR.Tester.App.Mef.Extension
{
    public static class MEFExtensions
    {
        public static IEnumerable<T> GetExportedValues<T>(this CompositionContainer Container,
            Func<IDictionary<string, object>, bool> Predicate)
        {
            var result = new List<T>();

            foreach (var PartDef in Container.Catalog.Parts)
            {
                foreach (var ExportDef in PartDef.ExportDefinitions)
                {
                    if (ExportDef.ContractName == typeof(T).FullName)
                    {
                        if (Predicate(ExportDef.Metadata))
                            result.Add((T)PartDef.CreatePart().GetExportedValue(ExportDef));
                    }
                }
            }

            return result;
        }

        public static IEnumerable<T> GetExportMetadataValues<T>(this CompositionContainer Container,
            Func<string, bool> Predicate)
        {
            var result = new List<T>();

            foreach (var PartDef in Container.Catalog.Parts)
            {
                foreach (var ExportDef in PartDef.ExportDefinitions)
                {
                    foreach (var MetadataDef in ExportDef.Metadata)
                        if (Predicate(MetadataDef.Key))
                            result.Add((T)MetadataDef.Value);
                }
            }

            return result;
        }
    }
}
