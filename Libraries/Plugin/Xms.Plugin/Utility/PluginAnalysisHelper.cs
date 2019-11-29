using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xms.Infrastructure;
using Xms.Plugin.Abstractions;
using Xms.Plugin.Domain;

namespace Xms.Plugin
{
    public static class PluginAnalysisHelper
    {
        public static List<Type> GetInstances()
        {
            List<Type> list = new List<Type>();
            list.Add(typeof(IPlugin<,>));
            list.Add(typeof(IEntityPlugin));
            return list;
        }

        public static PluginAnalyses Load(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Extension.ToLower() == ".dll")
                {
                    PluginAnalyses pluginAnalyses = new PluginAnalyses();
                    pluginAnalyses.FilePath = fileInfo.FullName;
                    pluginAnalyses.PluginAssembly = Assembly.Load(ReadFile(fileInfo.FullName).ToArray());
                    pluginAnalyses.Instances = GetInstances();

                    return pluginAnalyses;
                }
                else
                    throw new XmsException("文件类型不正确");
            }
            else
                throw new XmsException("未找到相关{" + filePath + "}文件");
        }

        public static MemoryStream ReadFile(string path)
        {
            MemoryStream memStream;
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                using (memStream = new MemoryStream())
                {
                    int res;
                    byte[] b = new byte[4096];
                    while ((res = stream.Read(b, 0, b.Length)) > 0)
                    {
                        memStream.Write(b, 0, b.Length);
                    }
                }
            }
            return memStream;
        }

        public static List<PluginAnalysis> GetPluginAnalysis(PluginAnalyses pluginAnalyses)
        {
            List<PluginAnalysis> plugins = new List<PluginAnalysis>();
            foreach (var dllModule in pluginAnalyses.PluginAssembly.GetLoadedModules())
            {
                foreach (var typeDefinedInModule in dllModule.GetTypes())
                {
                    if (typeDefinedInModule.IsClass)
                    {
                        var plugin = new PluginAnalysis();
                        plugin.IsPlugin = typeDefinedInModule.GetInterfaces().Contains(typeof(IAbsPlugin));
                        if (plugin.IsPlugin)
                        {
                            foreach (var _interface in pluginAnalyses.Instances)
                            {
                                if (typeDefinedInModule.GetInterfaces().Select(x => x.GUID).Contains(_interface.GUID))
                                    plugin.PluginInstances.Add(ConvertToInstanceInfo(_interface));
                            }
                        }

                        plugin.Plugin = ConvertToPluginInfo(typeDefinedInModule);

                        plugins.Add(plugin);
                    }
                }
            }
            return plugins;
        }

        public static List<PluginAnalysis> GetPluginAnalysis(string filePath)
        {
            return GetPluginAnalysis(Load(filePath));
        }

        public static PluginInfo ConvertToPluginInfo(Type type)
        {
            PluginInfo pluginInfo = new PluginInfo();
            pluginInfo.Name = type.Name;
            pluginInfo.Namespace = type.Namespace;
            pluginInfo.Assembly = ConvertToAssemblyInfo(type.Assembly);
            pluginInfo.MethodInfos = ConvertToMethodInfo(type.GetMethods());
            pluginInfo.Instances = ConvertToInstanceInfo(type.GetInterfaces());
            return pluginInfo;
        }

        public static AssemblyInfo ConvertToAssemblyInfo(Assembly assembly)
        {
            AssemblyInfo assemblyInfo = new AssemblyInfo()
            {
                FullName = assembly.FullName,
                CodeBase = assembly.CodeBase,
                EscapedCodeBase = assembly.EscapedCodeBase,
                GlobalAssemblyCache = assembly.GlobalAssemblyCache,
                HostContext = assembly.HostContext,
                ImageRuntimeVersion = assembly.ImageRuntimeVersion,
                IsDynamic = assembly.IsDynamic,
                IsFullyTrusted = assembly.IsFullyTrusted,
                Location = assembly.Location,
                ReflectionOnly = assembly.ReflectionOnly,
                SecurityRuleSet = assembly.SecurityRuleSet.ToString(),
                AssemblyName = assembly.GetName().Name + ".dll"
            };
            return assemblyInfo;
        }

        public static List<Domain.MethodInfo> ConvertToMethodInfo(System.Reflection.MethodInfo[] methodInfos)
        {
            List<Domain.MethodInfo> list = new List<Domain.MethodInfo>();
            foreach (var methodInfo in methodInfos)
            {
                list.Add(new Domain.MethodInfo
                {
                    Name = methodInfo.Name
                });
            }

            return list;
        }

        public static Domain.InstanceInfo ConvertToInstanceInfo(Type instance)
        {
            return new Domain.InstanceInfo
            {
                Name = string.IsNullOrWhiteSpace(instance.FullName) ? instance.Name : instance.FullName,
                GUID = instance.GUID
            };
        }

        public static List<Domain.InstanceInfo> ConvertToInstanceInfo(Type[] instances)
        {
            List<Domain.InstanceInfo> list = new List<Domain.InstanceInfo>();
            foreach (var instance in instances)
            {
                list.Add(ConvertToInstanceInfo(instance));
            }
            return list;
        }
    }
}