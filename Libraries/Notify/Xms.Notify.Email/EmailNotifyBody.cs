using System.Text;
using Xms.Notify.Abstractions;

namespace Xms.Notify.Email
{
    /// <summary>
    /// 邮件通知信息体
    /// </summary>
    public class EmailNotifyBody : NotifyBody
    {
        public string To { get; set; }

        /// <summary>
        /// 邮件服务器地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 邮件服务器端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 发送邮件的账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 发送邮件的密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 发送邮件
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// 发送邮件的昵称
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public Encoding BodyEncoding { get; set; } = Encoding.GetEncoding("utf-8");

        public bool IsBodyHtml { get; set; } = false;
    }
}