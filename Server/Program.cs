using Microsoft.AspNetCore.Authentication.Cookies;
using Server.Models.Data.Services;
using SignalRApp;
using Server.Hubs;
using Server.Models.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/auth/login";
        options.AccessDeniedPath = "/";
        options.SlidingExpiration = true;
    });
builder.Services.AddSingleton<ApplicationContext>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<ConferenceRoomService>();

var app = builder.Build();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}"
);
app.MapHub<WebRTCHub>("/webrtchub");
app.MapHub<ChatHub>("/chat");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}");



app.Run();
