using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Graph.Models;
using System.Net;

namespace MaxSys.Models
{
    public class Emai_Template_STMP
    {
        public List<Recipient_STMP> Recipient { get; set; }
        public List<Recipient_STMP> CC { get; set; }
        public List<Recipient_STMP> BCC { get; set; }
        public List<(string ori, string replace)> WORD_REPLACE { get; set; }
        public string Subject { get; set; }
        public string bodyContent { get; set; }
        public string mainTemplate { get; set; }
        public string subTemplate { get; set; }
        public Setting_Setup Setting_Setup { get; set; }
        public (bool, string) WordReplacer(string template)
        {
            try
            {
                if (WORD_REPLACE == null || !WORD_REPLACE.Any()) return (false, template);

                foreach (var (ori, replace) in WORD_REPLACE)
                {
                    if (!string.IsNullOrEmpty(ori) && template.Contains(ori))
                    {
                        template = template.Replace(ori, replace);
                    }
                }

                return (true, template);
            }
            catch (Exception ex)
            {
                return (false, template);
            }

        }
        public async Task<string> EmailBodyTemplate()
        {
            try
            {
                // Use Path.Combine to ensure proper path handling
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "template_email.html");

                string emailBody = "";
                if (System.IO.File.Exists(filePath))
                {
                    emailBody = await System.IO.File.ReadAllTextAsync(filePath);
                }

                return WebUtility.HtmlDecode(emailBody);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
    public class Emai_TemplateSent
    {
        public List<Recipient> Recipient { get; set; }

        public List<Recipient> CcRecipients { get; set; }
        public List<Recipient> CC { get; set; }
        public List<Recipient> BCC { get; set; }
        public List<(string ori, string replace)> WORD_REPLACE { get; set; }
        public string Subject { get; set; }
        public string bodyContent { get; set; }
        public string mainTemplate { get; set; }
        public string subTemplate { get; set; }
        public Setting_Setup Setting_Setup { get; set; }

        // New Attachments Property
        public List<EmailAttachment> Attachments { get; set; }

        public (bool, string) WordReplacer(string template)
        {
            try
            {
                if (WORD_REPLACE == null || !WORD_REPLACE.Any()) return (false, template);

                foreach (var (ori, replace) in WORD_REPLACE)
                {
                    if (!string.IsNullOrEmpty(ori) && template.Contains(ori))
                    {
                        template = template.Replace(ori, replace);
                    }
                }

                return (true, template);
            }
            catch (Exception ex)
            {
                return (false, template);
            }
        }
        public class EmailAttachment
        {
            public string FileName { get; set; }
            public byte[] FileContent { get; set; }
            public string ContentType { get; set; } // e.g., "application/pdf", "image/png"
        }
        public async Task<string> EmailBodyTemplate()
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "template_email.html");

                string emailBody = "";
                if (System.IO.File.Exists(filePath))
                {
                    emailBody = await System.IO.File.ReadAllTextAsync(filePath);
                }

                return WebUtility.HtmlDecode(emailBody);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public class Recipient_STMP {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
    }
}
