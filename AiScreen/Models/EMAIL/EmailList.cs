namespace MaxSystemWebSite.Models.EMAIL
{
    public class EmailList
    {
        public EmailList(string messageId, string conversationId, string from, List<string> to, string receivedDateTime, string subject, string body)
        {
            this.messageId = messageId;
            this.conversationId = conversationId;
            this.from = from;
            this.to = to;
            this.receivedDateTime = receivedDateTime;
            this.subject = subject;
            this.body = body;
        }

        public string messageId { get; set; }
        public string conversationId { get; set; }
        public string from { get; set; }
        public List<string> to { get; set; }
        public string receivedDateTime { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }
}
