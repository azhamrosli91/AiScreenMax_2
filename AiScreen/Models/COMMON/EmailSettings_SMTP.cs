namespace SmartTemplateCore.Models.Common
{
    public class EmailSettings_SMTP
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string ImapHost { get; set; }
        public int ImapPort { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SenderName { get; set; }
    }
}
