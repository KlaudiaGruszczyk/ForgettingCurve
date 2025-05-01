using ForgettingCurve.Application;
using ForgettingCurve.Infrastructure;
using Microsoft.OpenApi.Models;
using ForgettingCurve.Infrastructure.Email;
using ForgettingCurve.Application.Common.Interfaces;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja Rate Limitingu
builder.Services.AddRateLimiter(options =>
{
    // Domyślne limity dla wszystkich endpointów
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? context.Request.Headers.Host.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));

    // Bardziej restrykcyjne limity dla endpointów uwierzytelniania
    options.AddPolicy("AuthPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? context.Request.Headers.Host.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(5)
            }));

    // Obsługa przekroczenia limitu
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/problem+json";

        var problem = new
        {
            type = "rate_limit_error",
            title = "Too many requests",
            status = 429,
            detail = "You have exceeded the rate limit. Please try again later.",
            retryAfter = 60
        };

        await context.HttpContext.Response.WriteAsJsonAsync(problem, token);
    };
});

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

builder.Services
    .AddApplication()
    .AddInfrastructureServices(builder.Configuration);

builder.Services.Configure<MailtrapSettings>(builder.Configuration.GetSection("Mailtrap"));
builder.Services.AddScoped<IEmailService, MailtrapEmailAdapter>();

var app = builder.Build();

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
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

