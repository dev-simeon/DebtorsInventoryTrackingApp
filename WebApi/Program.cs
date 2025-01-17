
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Configurations;
using WebApi.Controllers;
using WebApi.Data;
using WebApi.Middleware;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Bind the AppSettings section to the Settings class
        var settings = builder.Configuration.GetSection("AppSettings").Get<Settings>()
            ?? throw new Exception("AppSettings configuration is missing or invalid. Check your appsettings.json or user-secrets.");

        // Add services to the container
        builder.Services.AddSingleton(settings);

        builder.Services.AddScoped<ControllerParameters>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddMemoryCache();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.CustomOperationIds(e =>
            {
                var controllerAction = (ControllerActionDescriptor)e.ActionDescriptor;
                return controllerAction.ActionName;
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" + " Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" + "Example: \"Bearer 1safsfsdfdfd\"",
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

        // Configure CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });

        // Configure Entity Framework Core
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(settings.ConnStrings.SqlDbLocal);
        });

        // Configure Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var key = Encoding.ASCII.GetBytes(settings.JWT.Secret);

            options.SaveToken = true;
            options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        var app = builder.Build();

        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Application started in {Environment} environment", app.Environment.EnvironmentName);

        // Add the global exception handler middleware
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        app.UseSwagger(options =>
        {
            options.RouteTemplate = "api-docs/{documentName}/openapi.json";
        });
        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = "DebtorsIventoryTracking OpenApi";
            options.SwaggerEndpoint("/api-docs/v1/openapi.json", "DebtorsIventoryTracking Api V1");
            options.RoutePrefix = string.Empty;
        });

        app.UseHttpsRedirection();
        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
