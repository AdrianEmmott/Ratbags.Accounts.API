using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ratbags.Account.Models;
using Ratbags.Account.ServiceExtensions;
using Ratbags.Core.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettingsBase>(builder.Configuration);
var appSettings = builder.Configuration.Get<AppSettingsBase>() ?? throw new Exception("Appsettings missing");
var certificatePath = string.Empty;
var certificateKeyPath = string.Empty;

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// are we in docker?
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

certificatePath = Path.Combine(appSettings.Certificate.Path, appSettings.Certificate.Name);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(Convert.ToInt32(appSettings.Ports.Http));
    serverOptions.ListenAnyIP(Convert.ToInt32(appSettings.Ports.Https), listenOptions =>
    {
        listenOptions.UseHttps(
            certificatePath,
            appSettings.Certificate.Password);
    });
});

// cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("https://localhost:5001") // ocelot
            .WithOrigins("https://localhost:5000") // ocelot http
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});


// add services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

// add service extensions
builder.Services.AddDIServiceExtension();
builder.Services.AddAuthenticationServiceExtension(appSettings);

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCookiePolicy();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();