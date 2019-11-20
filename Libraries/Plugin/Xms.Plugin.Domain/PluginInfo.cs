using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Xms.Plugin.Domain
{

    public class PluginInfo
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public AssemblyInfo Assembly{ get; set; }
        public List<MethodInfo> MethodInfos { get; set; }
        public  List<InstanceInfo> Instances { get; set; }
    }
}
