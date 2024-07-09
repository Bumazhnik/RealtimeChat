namespace RealtimeChat.DTO
{
    public class ChatSessionDTO
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public List<PublicUserDTO> Users {  get; set; }
    }
}
