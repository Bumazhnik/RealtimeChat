using Mapster;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealtimeChat.Db;
using RealtimeChat.Entities;
using RealtimeChat.Hubs;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(x => x.ListenAnyIP(80));

// Add services to the container.
builder.Services.AddControllersWithViews();
var config = JsonSerializer.Deserialize<SensitiveConfig>(File.ReadAllText("sensitiveConfig.json"));
if (config == null)
    throw new Exception("No config provided");
builder.Services.AddSingleton(config);
builder.Services.AddSingleton<IPasswordHasher<User>>(new PasswordHasher<User>());
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(config.ConnectionString));
builder.Services.AddSingleton<UserConnectionManager>();
builder.Services.AddMapster();
builder.Services.AddSignalR();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options => //CookieAuthenticationOptions
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/Login";
        });



var app = builder.Build();

/*using(var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApplicationContext>().Database.EnsureDeleted();
}*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Chat}/{action=Index}/{id?}");

app.Run();
