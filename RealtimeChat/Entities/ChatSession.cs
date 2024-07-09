﻿namespace RealtimeChat.Entities
{
    public class ChatSession
    {
        public int Id { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}