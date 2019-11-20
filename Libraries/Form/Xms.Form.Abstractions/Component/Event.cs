namespace Xms.Form.Abstractions.Component
{
    public sealed class Event
    {
        public string Name { get; set; }

        public bool Application { get; set; }

        public bool Active { get; set; }

        public string Attribute { get; set; }

        public string JsAction { get; set; }
        public string JsLibrary { get; set; }
    }

    public sealed class EventNames
    {
        public const string OnLoad = "onload";//表单加载事件
        public const string OnSave = "onsave";//表单保存事件
        public const string OnChange = "onchange";//字段更改事件
        public const string OnStateChange = "onstatechange";//tab展开收缩事件
    }
}