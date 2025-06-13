namespace SmartTemplateCore.Models.Common
{
    public class MessageChatBot
    {
        public MessageChatBot()
        {

        }

        public MessageChatBot(string _role, string _content)
        {
            role = _role;
            content = _content;
        }

        public string role { get; set; }
        public string content { get; set; }
    }
}
