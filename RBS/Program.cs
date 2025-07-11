using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RBS.Data;
using RBS.Services.Implenetation;
using RBS.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// ტოკენის კონფიგურაცია
builder.Services.AddScoped<IJWTService, JWTService>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "chven",
            ValidAudience = "isini",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("d47c9512f6e7de79efb7ea748d322ad9ada04290e752283e04400f27aeade5cbd5c49c8a5dce653cac523a2762656e6e3ee63bba1666df6ea0bb1276cf3b65248a37a09b2f40509a7d6e9ec60a3b7e3c68ca2d9eb60c41a36d3a42a523b44da2eb250d8706cccc4344b416dbb75131cccb64a49a30e564b0cd65de27df76d71312adcfc76decf23458ddcbd83dfcb1b0f60395a3fad3f6514f02df57ab2044422e20c04b886a996f08f885a5328c5324e6f1dd8e9bfa0b490516ccfd741eff5af5f0d7ba2372f332d0dc2bc47d0dbb1f87037de5bf1a578d07c90e0c811f137b5e8ca4f3bb2967d880c7363d17c6611fbfa6518feb82ba0249e0b40391f65e13")),
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("Universal", policy => policy.RequireRole("Owner, Admin"));
    
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();