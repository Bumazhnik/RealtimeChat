using System.Collections.Concurrent;

namespace RealtimeChat.Entities
{
    public class UserConnectionManager
    {
        /// <summary>
        /// user id, connection id
        /// </summary>
        public ConcurrentDictionary<int,ConcurrentSet<string>> ConnectedIds { get; private set; } = new();
    }
}
