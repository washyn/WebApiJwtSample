using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApi.Controllers;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const string apiVersion = "v1";
            
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.
            builder.Services.Configure<JwtBearer>(builder.Configuration.GetSection(nameof(JwtBearer)));

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        // The signing key must match!
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtBearer:SecurityKey"])),

                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = false,
                        ValidIssuer = builder.Configuration["JwtBearer:Issuer"],

                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = false,
                        ValidAudience = builder.Configuration["JwtBearer:Audience"],

                        // Validate the token expiry
                        ValidateLifetime = true,
                        
                        ClockSkew = TimeSpan.FromMinutes(0), // para que pasados 0 min expire el token...
                    };
                });
            
            
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // options.SwaggerDoc(apiVersion, new OpenApiInfo()
                // {
                //     Version = apiVersion,
                // });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
                {
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    Description = "",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = JwtBearerDefaults.AuthenticationScheme,
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();           
        }
    }
}