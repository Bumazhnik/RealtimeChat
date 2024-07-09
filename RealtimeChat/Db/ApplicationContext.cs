using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RealtimeChat.Entities;
using System.Data;

namespace RealtimeChat.Db
{
    public class ApplicationContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        private IPasswordHasher<User> hasher;
        SensitiveConfig config;
        public ApplicationContext(DbContextOptions<ApplicationContext> options,SensitiveConfig config,IPasswordHasher<User> hasher)
            : base(options)
        {
            this.config = config;
            this.hasher = hasher;
            Database.EnsureCreated();
        }
        private User MakeAdmin(Role adminRole)
        {
            string adminEmail = config.AdminMail;
            string adminPassword = config.AdminPassword;
            var user = new User { Email = adminEmail, Name = "admin", RoleId = adminRole.Id };
            string hashPassword = hasher.HashPassword(user, adminPassword);
            user.Password = hashPassword;
            return user;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";


            // adding roles
            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role userRole = new Role { Id = 2, Name = userRoleName };
            User adminUser = MakeAdmin(adminRole);
            User testUser = new User
            {
                Email = "test",
                Name = "test",
                RoleId = userRole.Id
            };
            adminUser.Id = 1;

            Message message = new Message
            {
                Id = 1,
                UserId = adminUser.Id,
            };


            modelBuilder.Entity<Role>().HasData([adminRole, userRole]);
            modelBuilder.Entity<User>().HasData([adminUser,testUser]);
            modelBuilder.Entity<ChatSession>().HasData();

            base.OnModelCreating(modelBuilder);
        }
    }
}
