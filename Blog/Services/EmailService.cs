using System;
using System.Net;
using System.Net.Mail;
using Blog.DTOs;

namespace Blog.Services
{
    public class EmailService
    {
        public bool Send(EmailDataDTO emailData)
        {
            var smtpClient = new SmtpClient(Environment.GetEnvironmentVariable("SMTP_HOST"), Int32.Parse(Environment.GetEnvironmentVariable("SMTP_PORT")));

            smtpClient.Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("SMTP_USERNAME"), Environment.GetEnvironmentVariable("SMTP_PASSWORD"));
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
                //smtpClient.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}