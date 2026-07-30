using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Thêm namespace này
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectBase.Helpers;
using ProjectBase.Health;
using ProjectBase.Configuration;
using ProjectBase.Middleware;
using ProjectBase.Models;
using ProjectBase.Services;
using System;
using System.Threading.RateLimiting;

namespace ProjectBase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsProduction())
            {
                ProductionConfigurationValidator.Validate(builder.Configuration);
            }

            builder.Logging.ClearProviders();
            if (builder.Environment.IsDevelopment())
            {
                builder.Logging.AddSimpleConsole(options =>
                    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ");
            }
            else
            {
                builder.Logging.AddJsonConsole(options =>
                    options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ");
            }

            // Configure services
            ConfigureServices(
                builder.Services,
                builder.Configuration,
                builder.Environment);

            var app = builder.Build();

            // Configure the HTTP request pipeline
            Configure(app, app.Environment);

            app.Run();
        }

        private static void ConfigureServices(
            IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            int passwordResetLinkExpirationHours = configuration.GetValue<int>("PasswordResetLinkExpirationHours");
            // Connection to the database
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ConnectedDb"));
            });

            // Add services to the container 
            services.AddControllersWithViews();
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.Headers.RetryAfter = "600";
                    context.HttpContext.Response.ContentType = "application/problem+json";
                    await System.Text.Json.JsonSerializer.SerializeAsync(
                        context.HttpContext.Response.Body,
                        new
                        {
                            status = StatusCodes.Status429TooManyRequests,
                            title = "Too many requests.",
                            detail = "Please wait before trying again."
                        },
                        cancellationToken: cancellationToken);
                };
                options.AddPolicy("AccountRegistration", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(10),
                            QueueLimit = 0
                        }));
                options.AddPolicy("Login", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(10),
                            QueueLimit = 0
                        }));
                options.AddPolicy("PasswordReset", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(10),
                            QueueLimit = 0
                        }));
            });
            services.AddOptions<EmailOptions>()
                .Bind(configuration.GetSection(EmailOptions.SectionName));
            services.AddSingleton<IAccountLinkBuilder, AccountLinkBuilder>();
            services.AddScoped<IEmailSender, SmtpEmailSender>();
            services.AddScoped<IPasswordHasher<ProjectBase.Models.DAO.User>, PasswordHasher<ProjectBase.Models.DAO.User>>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IPracticeCreationFaultInjector, NoOpPracticeCreationFaultInjector>();
            services.AddHealthChecks()
                .AddCheck(
                    "self",
                    () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(),
                    tags: ["live", "ready"])
                .AddCheck<DatabaseHealthCheck>(
                    "database",
                    tags: ["ready"]);

            // Add authentication services
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = environment.IsProduction()
                        ? CookieSecurePolicy.Always
                        : CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.SlidingExpiration = true;
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.Events.OnRedirectToLogin = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        }

                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            return Task.CompletedTask;
                        }

                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    };
                });

            // Add authorization services
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
                options.AddPolicy("MarketingOnly", policy => policy.RequireRole("Marketing"));
                options.AddPolicy("SaleOnly", policy => policy.RequireRole("Sale"));
                options.AddPolicy("ExpertOnly", policy => policy.RequireRole("Expert"));
                options.AddPolicy("GuestOnly", policy => policy.RequireRole("Guest"));
                options.AddPolicy("DashboardAccess", policy => policy.RequireRole("Marketing", "Admin"));
            });

            // Register IConfiguration for injection
            services.AddSingleton(configuration);
        }

        private static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseMiddleware<ApiExceptionHandlingMiddleware>();
            app.UseMiddleware<SecurityHeadersMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseRateLimiter();

            // Use authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = registration => registration.Tags.Contains("live"),
                    ResponseWriter = HealthCheckResponseWriter.WriteJsonAsync
                });
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = registration => registration.Tags.Contains("ready"),
                    ResponseWriter = HealthCheckResponseWriter.WriteJsonAsync
                });
            });
            
        }

    }
}
