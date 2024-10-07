using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ratbags.Account.Models;
using Ratbags.Account.ServiceExtensions;
using Ratbags.Core.Settings;
using Ratbags.Login.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettingsBase>(builder.Configuration);
var appSettings = builder.Configuration.Get<AppSettingsBase>() ?? throw new Exception("Appsettings missing");

// kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5077); // HTTP
    serverOptions.ListenAnyIP(7158, listenOptions =>
    {
        listenOptions.UseHttps(
            appSettings.Certificate.Name,
            appSettings.Certificate.Password);
    });
});

// cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("https://localhost:5001")      // ocelot
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDIServiceExtension();

// add basic identity but breakout when more complex
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthenticationExtension(appSettings);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
