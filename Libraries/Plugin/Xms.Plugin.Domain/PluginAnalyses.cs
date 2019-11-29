using System;
using System.Collections.Generic;
using System.Reflection;

namespace Xms.Plugin.Domain
{
    public class PluginAnalyses
    {
        public string FilePath { get; set; }
        public Assembly PluginAssembly { get; set; }
        public List<Type> Instances { get; set; }
    }
}