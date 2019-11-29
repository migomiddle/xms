using Xms.Core.Components.Platform;
using Xms.Form.Abstractions;

namespace Xms.RibbonButton.Abstractions
{
    public class CommandDefinition
    {
        public FormStateRules FormStateRules { get; set; }

        public ValueRules ValueRules { get; set; }
    }

    public class FormStateRules
    {
        public bool Enabled { get; set; }

        public bool Visibled { get; set; }

        public FormState[] States { get; set; }
    }

    public class ValueRules
    {
        public bool Enabled { get; set; }

        public bool Visibled { get; set; }

        public ValueRule[] Values { get; set; }
    }
}