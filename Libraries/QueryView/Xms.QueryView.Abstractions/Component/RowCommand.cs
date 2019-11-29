using Newtonsoft.Json.Converters;
using Xms.Sdk.Abstractions.Query;

namespace Xms.QueryView.Abstractions.Component
{
    public class RowCommand
    {
        public LogicalOperator LogicalOperator { get; set; }
        public ConditionExpression[] Conditions { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public RowCommandEventType EventType { get; set; }

        private RowCommandActionType _actionType;

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public RowCommandActionType ActionType
        {
            get { return _actionType; }
            set
            {
                _actionType = value;
                if (_actionType == RowCommandActionType.SetRowBackground)
                {
                    Action = new SetRowBackgroundAction();
                }
            }
        }

        public RowCommandAction Action { get; set; }

        public string CustomAction { get; set; }

        public string JsLibrary { get; set; }
    }

    public enum RowCommandEventType
    {
        OnBinding
    }

    public enum RowCommandActionType
    {
        SetRowBackground
    }

    public class RowCommandAction
    {
    }

    public class SetRowBackgroundAction : RowCommandAction
    {
        public string Color { get; set; }
    }
}