using DomainLayer.Contracts;
using E_Commerce.Web.Factories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using Persistence.Data;
using Persistence.Repositories;
using Service;
using Services;
using ServicesAbstraction;
using System.Text;
using System.Text.Json.Serialization;

namespace E_Commerce.Web.Extensions
{
    public static class ServicesRegisteration
    {
        public static IServiceCollection AddAuthorizationHeader(this IServiceCollection Services)
        {
            Services.AddEndpointsApiExplorer();
            Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "E-Commerce.Web", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                        new List<string>()
                    }
                });
            });
            return Services;
        }

        public static IServiceCollection AddWebApplicationServices(this IServiceCollection Services)
        {
            Services.Configure<ApiBehaviorOptions>(Options =>
            {
                Options.InvalidModelStateResponseFactory =
                    ApiResponseFactory.GenerateApiValidationErrorsResponse;
            });

            // Serialize enums as strings everywhere (request binding + response JSON)
            // ?sort=PriceAsc → bound correctly; response returns "PriceAsc" not 3
            Services.AddControllers()
                    .AddJsonOptions(o =>
                        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            return Services;
        }

        public static IServiceCollection AddJWTService(
            this IServiceCollection Services,
            IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("JWT:Options");
            var secretKey = jwtSection["SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];

            Services.AddAuthentication(config =>
            {
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = key
                };
            });

            // Authorization policies — registered ONCE here only
            Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("SuperAdmin", policy => policy.RequireRole("SuperAdmin"));
            });

            return Services;
        }
    }
}