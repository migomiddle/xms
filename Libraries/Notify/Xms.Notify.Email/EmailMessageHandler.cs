using System.Net;
using System.Net.Mail;
using Xms.Notify.Abstractions;

namespace Xms.Notify.Email
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
}