using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xms.Infrastructure.Utility;
using Xms.SiteMap.Domain;

namespace Xms.SiteMap
{
    /// <summary>
    /// 程序集助手服务
    /// </summary>
    public class AssemblyService
    {
        public static List<Privilege> GetAllActionByAssembly()
        {
            var result = new List<Privilege>();

            var types = Assembly.Load("Xms.Web").GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && !t.Name.IsCaseInsensitiveEqual("Controller")
            && !t.Name.IsCaseInsensitiveEqual("ControllerBase") && t.Name.EndsWith("Controller")).ToList();

            foreach (var type in types)
            {
                var members = type.GetMethods().Where(x => x.ReturnType.Name.EndsWith("ActionResult"));
                foreach (var member in members)
                {
                    if (result.Exists(n => n.ClassName.IsCaseInsensitiveEqual(member.DeclaringType.Name) && n.MethodName.IsCaseInsensitiveEqual(member.Name)))
                    {
                        continue;
                    }

                    object[] attrs = member.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true);
                    string desc = string.Empty;
                    if (attrs.Length > 0)
                    {
                        desc = (attrs[0] as System.ComponentModel.DescriptionAttribute).Description;
                    }

                    result.Add(new Privilege() { ClassName = member.DeclaringType.Name, MethodName = member.Name, DisplayName = desc });
                }
            }
            return result;
        }
    }
}