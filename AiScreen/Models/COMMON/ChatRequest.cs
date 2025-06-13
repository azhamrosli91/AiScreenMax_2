namespace SmartTemplateCore.Models.Common
{
    public class ChatRequest
    {
        public List<MessageChatBot> Messages { get; set; }
    }
    public class SnippaiChatRequest
    {
        public string TheadID { get; set; }
        public string UserName { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public List<MessageChatBot> Messages { get; set; }
    }
}
