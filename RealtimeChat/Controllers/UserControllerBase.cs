using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Db;
using RealtimeChat.Entities;

namespace RealtimeChat.Controllers
{
    public class UserControllerBase:Controller
    {
        protected readonly ApplicationContext db;

        public UserControllerBase(ApplicationContext db)
        {
            this.db = db;
        }

        protected async Task<User?> GetCurrentUser()
        {
            if (User.Identity?.Name == null) return null;
            return await db.Users.FirstOrDefaultAsync(x => x.Name == User.Identity.Name);
        }
    }
}
