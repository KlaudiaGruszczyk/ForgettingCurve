using ForgettingCurve.Application;
using ForgettingCurve.Infrastructure;
using Microsoft.AspNetCore.Identity;
using ForgettingCurve.Domain.Entities;
using ForgettingCurve.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;
using ForgettingCurve.Infrastructure.Email;
using ForgettingCurve.Application.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ForgettingCurve API",
        Version = "v1",
        Description = "API do zarządzania nauką z wykorzystaniem krzywej zapominania",
        Contact = new OpenApiContact
        {
            Name = "ForgettingCurve Team",
            Email = "support@forgettingcurve.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add application services
builder.Services
    .AddApplication()
    .AddInfrastructureServices(builder.Configuration);

builder.Services.Configure<MailtrapSettings>(builder.Configuration.GetSection("Mailtrap"));
builder.Services.AddScoped<IEmailService, MailtrapEmailAdapter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ForgettingCurve API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

