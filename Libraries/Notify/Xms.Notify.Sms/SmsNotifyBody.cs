using Xms.Notify.Abstractions;

namespace Xms.Notify.Sms
{
    /// <summary>
    /// 短信通知消息体
    /// </summary>
    public class SmsNotifyBody : NotifyBody
    {
        public string To { get; set; }
    }
}