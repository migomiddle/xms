using System;
using System.Collections.Generic;

namespace Xms.Form.Abstractions.Component
{
    public sealed class NavGroupDescriptor
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public bool IsVisible { get; set; } = true;

        public List<NavDescriptor> NavItems { get; set; }
    }
}