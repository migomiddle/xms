using System.Collections.Generic;
using Xms.Infrastructure;

namespace Xms.Dependency.Abstractions
{
    public class XmsDependencyException : XmsException
    {
        public List<DependentDescriptor> Dependents { get; set; }

        public XmsDependencyException(string message, List<DependentDescriptor> dependents) : base(500, message)
        {
            Dependents = dependents;
        }
    }
}