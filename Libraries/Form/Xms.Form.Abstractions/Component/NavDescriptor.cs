using System;

namespace Xms.Form.Abstractions.Component
{
    public sealed class NavDescriptor
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public bool IsVisible { get; set; } = true;

        public string Icon { get; set; }

        public string Url { get; set; }

        public string RelationshipName { get; set; }
    }
}