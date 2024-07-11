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

    private readonly IPasswordHasher<User> hasher;
    private readonly IMapper mapper;
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
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    public async Task<ActionResult<PublicUserDTO?>> MyUser()
    {
        var user = await GetCurrentUser();
        if (user == null)
            return BadRequest();
        return mapper.Map<PublicUserDTO>(user);
    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Name == model.Name);
            if (user == null)
            {
                // adding user to db
                user = new User {  Name = model.Name };
                string hashPass = hasher.HashPassword(user, model.Password);
                user.Password = hashPass;
                var userRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                if (userRole != null)
                    user.Role = userRole;

                db.Users.Add(user);
                await db.SaveChangesAsync();

                await Authenticate(user); // authentication

                return RedirectToAction("Index", "Chat");
            }
            else
                ModelState.AddModelError("", "Incorrect username and/or password");
        }
        return View(model);
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
                PasswordVerificationResult result = hasher.VerifyHashedPassword(user, user.Password ?? "", model.Password);
                switch (result)
                {
                    case PasswordVerificationResult.Success:
                        await Authenticate(user); // authentication

                        return RedirectToAction("Index", "Chat");
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
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name ?? ""),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name ?? "")
            };
        // making ClaimsIdentity object
        ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);
        // setting authentication cookie
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }
    


}

