using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MimeKit;
using MaxSys.Models;
using Microsoft.Graph.Models;
using MaxSystemWebSite.Models.SETTING;
using MaxSystemWebSite.Models.EMAIL;


namespace MaxSys.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(Emai_Template_STMP model);
    }
    public interface IEmail
    {
        void InitGraph(SETTING_EMAIL settings);
        Task<(bool success, string message, List<EmailList> data)> GetEmailListShort(
            string userID,
            string foldername = "inbox",
            string[] attribute = null,
            int count = 10,
            string[] orderby = null,
            DateTime? fromDate = null);

        Task<(bool success, string message, MessageCollectionResponse data)> GetEmailList(
             string userID,
             string foldername = "inbox",
             string[] attribute = null,
             int count = 10,
             string[] orderby = null);
        Task<(bool success, string message, List<EmailContent_Response>? data)> GetEmailBodyContentByConversationID(string userID, string conversationId);
        Task<(bool success, string message, List<EmailContent_Response>? data)> GetEmailBodyContentByMessageID(string userID, string messageId);
        Task<(bool success, string message, EmailContent_Response? data)> GetEmailBodyContentByV1MessageID(string userID, string messageId);
        Task<(bool success, string message)> SendEmailAsync(Emai_TemplateSent model);
        Task<(bool success, string message)> SendComposeEmailAsync(Emai_TemplateSent model);
    }
}
