using System.Net;
using System.Net.Mail;
using Blog.DTOs;
using Blog.Settings;

namespace Blog.Services
{
    public class EmailService
    {
        public bool Send(EmailDataDTO emailData, SmtpConfiguration smtp)
        {
            var smtpClient = new SmtpClient(smtp.Host, smtp.Port);

            smtpClient.Credentials = new NetworkCredential(smtp.UserName, smtp.Password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;

            var mail = new MailMessage();

            mail.From = new MailAddress(emailData.FromEmail, emailData.FromName);
            mail.To.Add(new MailAddress(emailData.ToEmail, emailData.ToName));
            mail.Subject = emailData.Subject;
            mail.Body = emailData.Body;
            mail.IsBodyHtml = true;

            try
            {
                smtpClient.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}