using Examination_System.Data.UnitOfWorks;
using Examination_System.Data;
using Examination_System.MapConfig;
using Examination_System.Models;
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
    builder.Services.AddScoped<UnitOfWork>();
    builder.Services.AddDbContext<Exam_sysContext>(op => op.UseSqlServer(builder.Configuration.GetConnectionString("Exam_SysContext")));

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(options =>
       {
           options.LoginPath = "/Auth/Login";
           options.AccessDeniedPath = "/Auth/AccessDenied"; // Changed to Auth controller
           options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
           options.SlidingExpiration = true;
           options.Cookie.HttpOnly = true;
           options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Added security
           options.Cookie.SameSite = SameSiteMode.Strict;
       });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("InstructorOnly", policy => policy.RequireRole("Instructor"));
        options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
        options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapStaticAssets();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                try
                {
                    logger.LogInformation("Initializing database...");
                    await SeedData.Initialize(services);
                    logger.LogInformation("Database initialization completed");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Database initialization failed");
                }
            }


            await app.RunAsync();
}
    }
}