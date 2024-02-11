using DynamicDNSViaCloudFlare.Models;
using System.Net;
using System.Net.Mail;

namespace DynamicDNSViaCloudFlare.Helpers
{
    public class EmailHelper
    {
        private static EmailSettings emailSettings { get; set; } = null;
        public EmailHelper(EmailSettings settings)
        {
            emailSettings = settings;
        }
        public void SendEmail(string Subject, string Message, string[] ToAddressList)
        {
            var smtpClient = new SmtpClient
            {
                Host = emailSettings.OutgoingServer.Server,
                Port = emailSettings.OutgoingServer.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailSettings.OutgoingServer.UserName, emailSettings.OutgoingServer.Password),
                EnableSsl = emailSettings.OutgoingServer.HasSSL,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings.OutgoingServer.FromAddress),
                Subject = Subject,
                Body = Message,
                IsBodyHtml = true,
            };
            foreach (var item in ToAddressList)
            {
                string[] add = item.Split(",");
                foreach (string item1 in add)
                {
                    mailMessage.To.Add(new MailAddress(item1));
                }
            }
            smtpClient.Send(mailMessage);
        }
        public void SendEmail(string Subject, string Message, string ToAddress = null)
        {
            ToAddress = string.IsNullOrEmpty(ToAddress) ? emailSettings.ReportTo : ToAddress;
            SendEmail(Subject, Message, new string[] { ToAddress });
        }
    }
}
