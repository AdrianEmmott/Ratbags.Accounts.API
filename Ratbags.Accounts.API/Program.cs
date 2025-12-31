using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ratbags.Accounts.API.Models;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Accounts.ServiceExtensions;

var builder = WebApplication.CreateBuilder(args);

// secrets
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.Configure<AppSettings>(builder.Configuration);
var appSettings = builder.Configuration.Get<AppSettings>() ?? throw new Exception("Appsettings missing");
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

// add services
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

builder.Services.AddSingleton(serviceProvider =>
{
    return new ServiceBusClient(appSettings.Messaging.ASB.Connection);
});

builder.Services.Configure<JsonOptions>(x =>
{
    x.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
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