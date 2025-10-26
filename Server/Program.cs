using Microsoft.AspNetCore.Authentication.Cookies;
using Server.Models.Data.Services;
using SignalRApp;
using Server.Hubs;
using Server.Models.Data;
using Microsoft.Extensions.DependencyInjection;

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
builder.Services.AddSingleton(provider => new UserService());
builder.Services.AddSingleton(provider => new ConferenceRoomService());

var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}"
);
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<WebRTCHub>("/webrtchub");
    // Уберите регистрацию ChatHub если он больше не нужен
    // endpoints.MapHub<ChatHub>("/chat");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}");



app.Run();
