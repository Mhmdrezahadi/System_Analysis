using Entities.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System_Analysis.Models;
using System_Analysis.Services;

namespace System_Analysis
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServiceCollection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=System_Analaysis;Trusted_Connection=True");
            });

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMemoryCache();

            services.AddScoped<IGlobalService, GlobalService>();

            services.AddSignalR();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                jwt.TokenValidationParameters =
                new TokenValidationParameters
                {
                    LifetimeValidator = (before, expires, token, param) =>
                    {
                        return expires > DateTime.UtcNow;
                    },

                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateActor = false,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["Tokens:Issuer"],
                    ValidAudience = configuration["Tokens:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            configuration["Tokens:Key"]))
                };

                jwt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;

                        if
                        (
                        !string.IsNullOrEmpty(accessToken)
                        &&
                        path.StartsWithSegments("/hub")
                        )
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
            services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 1;

                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                // Default User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "bale api",
                    Description = "راهنمای استفاده از رابط برنامه نویسی پیام رسان بله",
                    TermsOfService = new Uri("https://bale.ai/terms"),

                    Contact = new OpenApiContact
                    {
                        Name = "Bale co",
                        Email = "info@bale.com",
                        Url = new Uri("https://bale.ai"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "مجوز استفاده",
                        Url = new Uri("https://bale.ai/license"),
                    }
                });

                //c.DocumentFilter<LowerCaseDocumentFilter>();

                c.OrderActionsBy((apiDesc) =>
                {
                    return $"{apiDesc.RelativePath.Split('/')[3]}_{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}_{apiDesc.RelativePath}";
                }
               );

                c.DocInclusionPredicate((name, api) => true);

                var securityScheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Description =
                    @"JWT Authorization header using the Bearer scheme." +
                    "\r\n\r\n" +
                    "Enter TOKEN in the text input below." +
                    "\r\n\r\n" +
                    "Example: 'a1.b2.c3'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    BearerFormat = "JWT",

                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition("Bearer", securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {securityScheme, new[] { "Bearer" } }
                };

                c.AddSecurityRequirement(securityRequirement);

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);

                c.UseInlineDefinitionsForEnums();
            });
        }

    }
}
