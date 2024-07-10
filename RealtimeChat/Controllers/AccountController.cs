using MapsterMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Db;
using RealtimeChat.DTO;
using RealtimeChat.Entities;
using RealtimeChat.Models;
using System.Security.Claims;

namespace RealtimeChat.Controllers;

public class AccountController:UserControllerBase
{

    IPasswordHasher<User> hasher;
    IMapper mapper;
    public AccountController(ApplicationContext _db, IPasswordHasher<User> _hasher,IMapper mapper):base(_db)
    {
        hasher = _hasher;
        this.mapper = mapper;
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email || u.Name == model.Name);
            if (user == null)
            {
                // adding user to db
                user = new User { Email = model.Email, Name = model.Name };
                string hashPass = hasher.HashPassword(user, model.Password);
                user.Password = hashPass;
                var userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                if (userRole != null)
                    user.Role = userRole;

                db.Users.Add(user);
                await db.SaveChangesAsync();

                await Authenticate(user); // authentication

                return RedirectToAction("Index", "Home");
            }
            else
                ModelState.AddModelError("", "Incorrect username and/or password");
        }
        return View(model);
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Name == model.Name);
            if (user != null)
            {
                PasswordVerificationResult result = hasher.VerifyHashedPassword(user, user.Password, model.Password);
                switch (result)
                {
                    case PasswordVerificationResult.Success:
                        await Authenticate(user); // authentication

                        return RedirectToAction("Index", "Home");

                    case PasswordVerificationResult.SuccessRehashNeeded:
                        user.Password = hasher.HashPassword(user, model.Password);
                        db.Entry(user).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        goto case PasswordVerificationResult.Success;
                }
            }
            ModelState.AddModelError("", "Incorrect username and/or password");
        }
        return View(model);
    }
    private async Task Authenticate(User user)
    {
        // making one claim
        var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
        // making ClaimsIdentity object
        ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
        // setting authentication cookie
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }
    
    public async Task<PublicUserDTO?> MyUser()
    {
        var user = await GetCurrentUser();
        return mapper.Map<PublicUserDTO>(user);
    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
    public async Task<IActionResult> Init()
    {
        if (await db.ChatSessions.AnyAsync())
            return Ok("Already made");
        var admin = await db.Users.FirstOrDefaultAsync(x=>x.Id == 1);
        if (admin == null)
            return BadRequest();
        var testUser = await db.Users.FirstOrDefaultAsync(x=> x.Id == 2);
        if (testUser == null)
            return BadRequest();

        for (var i = 0; i < 10; i++)
        {
            var msg = new Message
            {
                Date = DateTime.Now,
                User = admin,
                Text = "Hello, World! From admin and random " + Random.Shared.Next(1,10001)
            };
            var msg1 = new Message
            {
                Date = DateTime.Now,
                User = testUser,
                Text = "Hello from test user! random " + Random.Shared.Next(1, 10001)
            };
            var session = new ChatSession
            {
                Name = "My group chat " + i,
                Messages = [msg,msg1],
                OwnerId = admin.Id,
                Users = [admin, testUser]
            };
            db.ChatSessions.Add(session);
        }
        await db.SaveChangesAsync();
        return Ok("Success");
    }
}

