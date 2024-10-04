using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ratbags.Account.Models;
using Ratbags.Account.ServiceExtensions;
using Ratbags.Login.Controllers;
using Ratbags.Login.Models;
using Ratbags.Shared.DTOs.Events.AppSettingsBase;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettingsBase>(builder.Configuration);
var appSettings = builder.Configuration.Get<AppSettingsBase>() ?? throw new Exception("Appsettings missing");

// config cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            //.WithOrigins("https://localhost:7117")    // ocelot - vs
            .WithOrigins("https://localhost:5001")      // ocelot - docker
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// add services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDIServiceExtension();

// config identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// authentication service
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
})
.AddGoogle(options =>
{
    options.ClientId = "your-google-client-id";
    options.ClientSecret = "your-google-client-secret";
})
.AddFacebook(options =>
{
    options.AppId = "your-facebook-app-id";
    options.AppSecret = "your-facebook-app-secret";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
