using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Xms.Infrastructure.Utility
{
    public class AssemblyHelper
    {
        public static List<Assembly> GetAssemblies(string searchPattern = "")
        {
            List<Assembly> assemblies = new List<Assembly>();
            if (searchPattern.HasValue())
            {
                DirectoryInfo root = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                foreach (FileInfo f in root.GetFiles(searchPattern))
                {
                    assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(f.FullName));
                }
            }
            else
            {
                assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            }
            return assemblies;
        }

        public static List<Type> GetClassOfType(Type assignTypeFrom, string searchPattern = "")
        {
            var assemblies = GetAssemblies(searchPattern);
            var result = new List<Type>();
            try
            {
                foreach (var a in assemblies)
                {
                    Type[] types = a.GetTypes();

                    if (types == null)
                    {
                        continue;
                    }

                    foreach (var t in types)
                    {
                        if (!assignTypeFrom.IsAssignableFrom(t) && (!assignTypeFrom.IsGenericTypeDefinition || !DoesTypeImplementOpenGeneric(t, assignTypeFrom)))
                        {
                            continue;
                        }

                        if (t.IsInterface)
                        {
                            continue;
                        }

                        if (t.IsAbstract)
                        {
                            continue;
                        }

                        result.Add(t);
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                var msg = string.Empty;
                foreach (var e in ex.LoaderExceptions)
                {
                    msg += e.Message + Environment.NewLine;
                }

                var fail = new Exception(msg, ex);
                Debug.WriteLine(fail.Message, fail);

                throw fail;
            }

            return result;
        }

        public static bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                foreach (var implementedInterface in type.FindInterfaces((objType, objCriteria) => true, null))
                {
                    if (!implementedInterface.IsGenericType)
                    {
                        continue;
                    }

                    var isMatch = genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());
                    return isMatch;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}