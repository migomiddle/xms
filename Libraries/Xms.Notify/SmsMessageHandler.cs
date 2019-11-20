using System;

namespace Xms.Notify
{
    /// <summary>
    /// 短信消息处理
    /// </summary>
    public class SmsMessageHandler : INotify
    {

        public SmsMessageHandler()
        {
        }

        public object Send(NotifyBody body)
        {
            var ibody = body as SmsNotifyBody;
            return null;
        }
    }

    public class SmsNotifyBody : NotifyBody
    {
        public string To { get; set; }
    }
}