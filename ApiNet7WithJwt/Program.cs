using Microsoft.EntityFrameworkCore;
using ApiNet7WithJwt.Models;
using ApiNet7WithJwt.Services;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Sql connection
builder.Services.AddDbContext<AuthJwtDbContext>(options =>
{
  options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});

// Add Jwt Pt1
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();

var key = builder.Configuration.GetValue<string>("JwtSettings:Key");
var keyBytes = Encoding.ASCII.GetBytes(key);

builder.Services.AddAuthentication(config =>
{
  config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
  config.RequireHttpsMetadata = false; // Deshabilita https
  config.SaveToken = true; // Guardar token
  config.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero, // No debe existir desviacion en el tiempo de vida del token
  };
});

// Enable Cors Pt1
var provider = builder.Services.BuildServiceProvider();
var configuration = provider.GetRequiredService<IConfiguration>();

builder.Services.AddCors(options =>
{
  var frontEndUrl = configuration.GetValue<string>("FrontEndUrl");


  options.AddDefaultPolicy(builder =>
  {
    builder.WithOrigins(frontEndUrl).AllowAnyHeader().AllowAnyMethod();
  });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

// Enable Cors Pt2
app.UseCors();

// Add Jwt Pt2
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
