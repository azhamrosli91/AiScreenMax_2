using Microsoft.Graph.Models;

namespace MaxSystemWebSite.Models.EMAIL
{
    public class EmailContent_Response
    {
        public string? Id { get; set; }
        public List<Attachment>? Attachments { get; set; }
        public List<AttachmentDto>? AttachmentDto { get; set; }
        public List<Recipient>? BccRecipients { get; set; }
        public string? Content { get; set; }
        public BodyType? ContentType { get; set; }
        public List<Recipient>? CcRecipients { get; set; }
        public string? ConversationId { get; set; }
        public Recipient? From { get; set; }
        public bool? HasAttachments { get; set; }
        public bool? IsRead { get; set; }
        public DateTimeOffset? ReceivedDateTime { get; set; }
        public string ReceivedDateTimeDesc { get; set; }
        public List<Recipient>? ReplyTo { get; set; }
        public Recipient? Sender { get; set; }
        public DateTimeOffset? SentDateTime { get; set; }
        public string? Subject { get; set; }
        public List<Recipient>? ToRecipients { get; set; }
    }
    public class AttachmentDto
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string ContentBytes { get; set; } // base64 string
    }
}
