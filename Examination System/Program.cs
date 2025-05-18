using Examination_System.Data.UnitOfWorks;
using Examination_System.Data;
using Examination_System.MapConfig;
using Examination_System.Models;

using Examination_System.Services;

using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.EntityFrameworkCore;

namespace Examination_System
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(typeof(MappingConfig));
            builder.Services.AddScoped<IExamGenerationService, ExamGenerationService>();

            builder.Services.AddScoped<UnitOfWork>();
            builder.Services.AddDbContext<Exam_sysContext>(op =>
                op.UseSqlServer(builder.Configuration.GetConnectionString("Exam_SysContext")));

            // Enhanced Authentication Configuration
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie(options =>
               {
                   options.LoginPath = "/Auth/Login";
                   options.AccessDeniedPath = "/Auth/AccessDenied";
                   options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                   options.SlidingExpiration = true;
                   options.Cookie.HttpOnly = true;
                   options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                   options.Cookie.SameSite = SameSiteMode.Strict;
               });

            // Enhanced Authorization Policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("InstructorOnly", policy => policy.RequireRole("Instructor"));
                options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));

                // New policies for permission management
                options.AddPolicy("BranchManagerOnly", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("Admin") ||
                        context.User.IsInRole("BranchManager")));

                options.AddPolicy("DepartmentManagerOnly", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("Admin") ||
                        context.User.IsInRole("BranchManager") ||
                        context.User.IsInRole("DepartmentManager")));

                // Combined manager policy
                options.AddPolicy("ManagerOnly", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("Admin") ||
                        context.User.IsInRole("BranchManager") ||
                        context.User.IsInRole("DepartmentManager")));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Database seeding
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    logger.LogInformation("Initializing database...");
                    await SeedData.Initialize(services);

                    // Ensure manager roles exist
                    var context = services.GetRequiredService<Exam_sysContext>();
                    if (!await context.Roles.AnyAsync(r => r.RoleTitle == "BranchManager"))
                    {
                        context.Roles.Add(new Role { RoleTitle = "BranchManager", isActive = true });
                    }
                    if (!await context.Roles.AnyAsync(r => r.RoleTitle == "DepartmentManager"))
                    {
                        context.Roles.Add(new Role { RoleTitle = "DepartmentManager", isActive = true });
                    }
                    await context.SaveChangesAsync();

                    logger.LogInformation("Database initialization completed");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Database initialization failed");
                }
            }
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Student}/{action=Index}/{id?}")
                .WithStaticAssets();

            await app.RunAsync();

        }
    }
}


