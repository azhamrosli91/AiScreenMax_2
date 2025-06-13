namespace E_Template.Services
{
    using MaxSys.Interface;
    using MailKit.Net.Imap;
    using MailKit.Net.Smtp;
    using MailKit.Search;
    using Microsoft.Graph.Models;
    using MimeKit;
    using SmartTemplateCore.Models.Common;
    using System.Net.Mail;
    using MaxSys.Models;
    using SmtpClient = MailKit.Net.Smtp.SmtpClient;

    public class EmailService_STMP : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService_STMP(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(Emai_Template_STMP model)
        {
            try
            {
                if (model == null)
                {
                    return false;
                }
                EmailSettings_SMTP _settings = new EmailSettings_SMTP();
                _settings.SmtpHost = _configuration["EmailSettings_SMTP:SmtpHost"];
                _settings.SmtpPort = int.Parse(_configuration["EmailSettings_SMTP:SmtpPort"]);
                _settings.ImapHost = _configuration["EmailSettings_SMTP:ImapHost"];
                _settings.ImapPort = int.Parse(_configuration["EmailSettings_SMTP:ImapPort"]);
                _settings.Email = _configuration["EmailSettings_SMTP:Email"];
                _settings.Password = _configuration["EmailSettings_SMTP:Password"];
                _settings.SenderName = _configuration["EmailSettings_SMTP:SenderName"];


                model.mainTemplate = await model.EmailBodyTemplate();
                model.bodyContent = model.mainTemplate.ToString().Replace("[BODY]", model.subTemplate);
                (bool success, string template) returnTemp = model.WordReplacer(model.bodyContent);
                if (returnTemp.success == false)
                {
                    return false;
                }
                model.bodyContent = returnTemp.template;

                //var message = new MimeMessage
                //{
                //    Subject = model.Subject,
                //    Body = new ItemBody
                //    {
                //        ContentType = BodyType.Html, // Change to HTML content type
                //        Content = model.bodyContent
                //    },
                //    ToRecipients = model.Recipient ?? new List<Recipient>(),
                //    CcRecipients = model.CC ?? new List<Recipient>(),
                //    BccRecipients = model.BCC ?? new List<Recipient>()
                //};

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.Email));
                if (model.Recipient != null && model.Recipient.Count > 0)
                {
                    foreach (Recipient_STMP item in model.Recipient)
                    {
                        if (item.EmailAddress != null && !string.IsNullOrEmpty(item.EmailAddress))
                        {
                            message.To.Add(new MailboxAddress(item.Name ?? string.Empty, item.EmailAddress));
                        }
                    }
                }
                if (model.CC != null && model.CC.Count > 0)
                {
                    foreach (Recipient_STMP item in model.CC)
                    {
                        if (item.EmailAddress != null && !string.IsNullOrEmpty(item.EmailAddress))
                        {
                            message.Cc.Add(new MailboxAddress(item.Name ?? string.Empty, item.EmailAddress));
                        }
                    }
                }
                if (model.BCC != null && model.BCC.Count > 0)
                {
                    foreach (Recipient_STMP item in model.BCC)
                    {
                        if (item.EmailAddress != null && !string.IsNullOrEmpty(item.EmailAddress))
                        {
                            message.Bcc.Add(new MailboxAddress(item.Name ?? string.Empty, item.EmailAddress));
                        }
                    }
                }

                message.Subject = model.Subject;
                message.Body = new TextPart("html")
                {
                    Text = model.bodyContent
                };

                using var smtpClient = new SmtpClient();
                await smtpClient.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, true);
                await smtpClient.AuthenticateAsync(_settings.Email, _settings.Password);
                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }

        //public async Task<IEnumerable<EmailMessage>> ReceiveEmailsAsync()
        //{
        //    var emails = new List<EmailMessage>();

        //    using var imapClient = new ImapClient();
        //    await imapClient.ConnectAsync(_settings.ImapHost, _settings.ImapPort, true);
        //    await imapClient.AuthenticateAsync(_settings.Email, _settings.Password);

        //    var inbox = imapClient.Inbox;
        //    await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

        //    foreach (var uid in await inbox.SearchAsync(SearchQuery.All))
        //    {
        //        var message = await inbox.GetMessageAsync(uid);
        //        emails.Add(new EmailMessage
        //        {
        //            Subject = message.Subject,
        //            From = message.From.ToString(),
        //            Body = message.TextBody,
        //            Date = message.Date.UtcDateTime
        //        });
        //    }

        //    await imapClient.DisconnectAsync(true);

        //    return emails;
        //}
    }

}
