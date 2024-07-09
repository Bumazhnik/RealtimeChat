using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
        public DbSet<Message> Messages { get; set; }
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
            adminUser.Id = 1;
            User testUser = new User
            {
                Id = 2,
                Email = "test",
                Name = "test",
                RoleId = userRole.Id
            };
            testUser.Password = hasher.HashPassword(testUser, config.AdminPassword);



            modelBuilder.Entity<Role>().HasData([adminRole, userRole]);
            modelBuilder.Entity<User>().HasData([adminUser,testUser]);

            base.OnModelCreating(modelBuilder);
        }
    }
}
