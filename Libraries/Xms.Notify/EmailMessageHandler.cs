using System.Net;
using System.Net.Mail;
using System.Text;

namespace Xms.Notify
{
    /// <summary>
    /// 邮件消息处理
    /// </summary>
    public class EmailMessageHandler : INotify
    {

        public EmailMessageHandler()
        {
        }

        public object Send(NotifyBody body)
        {
            var ibody = body as EmailNotifyBody;
            SmtpClient smtp = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = ibody.Host,
                Port = ibody.Port,
                Credentials = new NetworkCredential(ibody.UserName, ibody.Password)
            };
            if (ibody.Port != 25)
            {
                smtp.EnableSsl = true;
            }

            MailMessage mm = new MailMessage
            {
                Priority = MailPriority.Normal,
                From = new MailAddress(ibody.From, ibody.Subject, ibody.BodyEncoding),
                Subject = ibody.Subject,
                Body = ibody.Content,
                BodyEncoding = ibody.BodyEncoding,
                IsBodyHtml = ibody.IsBodyHtml
            };
            mm.To.Add(ibody.To);

            smtp.Send(mm);
            return true;
        }
    }

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