using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Db;
using RealtimeChat.DTO;
using RealtimeChat.Entities;
using System.Collections.Concurrent;

namespace RealtimeChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationContext db;
        private readonly UserConnectionManager manager;
        public ChatHub(ApplicationContext db, UserConnectionManager manager)
        {
            this.db = db;
            this.manager = manager;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await GetCurrentUser();
            if (user != null)
                manager.ConnectedIds
                    .AddOrUpdate(
                    user.Id, 
                    new ConcurrentSet<string>(Context.ConnectionId), 
                    (key,oldValue) =>
                    {
                        oldValue.Add(Context.ConnectionId);
                        return oldValue;
                    });
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var key = manager.ConnectedIds.FirstOrDefault(x=>x.Value.ContainsKey( Context.ConnectionId));
            if(key.Value != null)
            {
                key.Value.Remove(Context.ConnectionId);
                if (key.Value.Count <= 0)
                    manager.ConnectedIds.Remove(key.Key, out var _);
            }

            return base.OnDisconnectedAsync(exception);
        }
        private async Task<User?> GetCurrentUser()
        {
            if (Context.User?.Identity?.Name == null) return null;
            return await db.Users.FirstOrDefaultAsync(x => x.Name == Context.User.Identity.Name);
        }
    }
}
