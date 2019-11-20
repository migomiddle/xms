using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Xms.Plugin.Domain
{

    public class PluginAnalyses
    {
        public string FilePath { get; set; }
        public Assembly PluginAssembly { get; set; }
        public List<Type> Instances { get; set; }
    }
}
