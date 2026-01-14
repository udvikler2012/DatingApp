using System.Text;
using Api.Data;
using Api.Entities;
using Api.Helpers;
using Api.Interfaces;
using Api.Middleware;
using Api.Services;
using Api.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
   opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<LogUserActivity>();
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSignalR();
builder.Services.AddSingleton<PresenceTracker>();

builder.Services.AddIdentityCore<AppUser>(opt =>
{
   opt.Password.RequireNonAlphanumeric = false;
   opt.User.RequireUniqueEmail = true;

})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
   var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("Token key not found");
   options.TokenValidationParameters = new TokenValidationParameters
   {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
      ValidateIssuer = false,
      ValidateAudience = false
   };

   // Handling authentication for SignalR
   // SignalR uses WebSocket and sends access_token as a QueryString
   options.Events = new JwtBearerEvents
   {
      OnMessageReceived = context =>
      {
         var accessToken = context.Request.Query["access_token"];
         var path = context.HttpContext.Request.Path;
         if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
         {
            context.Token = accessToken;
         }
         return Task.CompletedTask;
      }
   };

});
builder.Services.AddAuthorizationBuilder()
   .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
   .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader()
   .AllowAnyMethod()
   .AllowCredentials()
   .WithOrigins("http://localhost:4200", "https://localhost:4200"));
app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/messages");
app.MapFallbackToController("Index","Fallback"); // Angular controller

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
   var context = services.GetRequiredService<AppDbContext>();
   var userManager = services.GetRequiredService<UserManager<AppUser>>();
   await context.Database.MigrateAsync();
   await context.Connections.ExecuteDeleteAsync();
   await Seed.SeedUsers(userManager);
}
catch (Exception ex)
{
   var logger = services.GetRequiredService<ILogger<Program>>();
   logger.LogError(ex, "An error occurred during migration");
}

app.Run();
