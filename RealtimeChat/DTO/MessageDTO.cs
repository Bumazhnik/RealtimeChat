
namespace RealtimeChat.DTO
{
    public class MessageDTO
    {
        public string UserName { get; set; }
        public int ChatSessionId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
