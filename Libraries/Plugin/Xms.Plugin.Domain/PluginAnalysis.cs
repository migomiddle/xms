using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Xms.Plugin.Domain
{

    public class PluginAnalysis
    {
        public PluginInfo Plugin { get; set; }
        public bool IsPlugin { get; set; }
        public List<InstanceInfo> PluginInstances { get; set; } = new List<InstanceInfo>();

    }
}
