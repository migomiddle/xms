using Xms.Notify.Abstractions;

namespace Xms.Notify.Sms
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
}