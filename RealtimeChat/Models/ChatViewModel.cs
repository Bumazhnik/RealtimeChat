using RealtimeChat.DTO;
using RealtimeChat.Entities;

namespace RealtimeChat.Models
{
    public class ChatViewModel
    {
        public PublicUserDTO CurrentUser { get; set; }
        public List<ChatSessionDTO> Sessions { get; set; }
    }
}
