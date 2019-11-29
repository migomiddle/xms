using System;

namespace Xms.Solution.Abstractions
{
    /// <summary>
    /// 解决方案组件导入节点标识
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SolutionImportNodeAttribute : Attribute
    {
        public SolutionImportNodeAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}