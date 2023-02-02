namespace Blog.DTOs
{
    public class EmailDataDTO
    {
        public EmailDataDTO(string toEmail, string toName, string subject, string body, string fromEmail, string fromName)
        {
            ToEmail = toEmail;
            ToName = toName;
            Subject = subject;
            Body = body;
            FromEmail = fromEmail;
            FromName = fromName;
        }

        public EmailDataDTO(string toEmail, string toName, string subject, string body)
        {
            ToEmail = toEmail;
            ToName = toName;
            Subject = subject;
            Body = body;
            FromEmail = "email@balta.io";
            FromName = "Curso ASP.NET";
        }

        public string ToEmail { get; private set; }
        public string ToName { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public string FromEmail { get; private set; }
        public string FromName { get; private set; }
    }
}