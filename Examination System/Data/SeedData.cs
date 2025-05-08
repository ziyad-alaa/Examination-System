using Examination_System.Models;
using Examination_System.Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Examination_System.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger("DatabaseSeeder");

            try
            {
                logger.LogInformation("Starting database initialization...");

                using var scope = serviceProvider.CreateScope();
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<Exam_sysContext>();
                var config = services.GetRequiredService<IConfiguration>();

                // Apply pending migrations
                await ApplyMigrations(context, logger);

                // Seed core system data
                await SeedCoreData(context, logger);

                // Ensure admin account exists
                await EnsureAdminAccount(context, config, logger);

                logger.LogInformation("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database initialization failed");
                throw;
            }
        }

        private static async Task ApplyMigrations(Exam_sysContext context, ILogger logger)
        {
            try
            {
                logger.LogInformation("Applying database migrations...");
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

                if (pendingMigrations.Any())
                {
                    logger.LogInformation($"Applying {pendingMigrations.Count()} migrations...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migrations applied successfully");
                }
                else
                {
                    logger.LogInformation("Database is up to date - no migrations to apply");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to apply database migrations");
                throw;
            }
        }

        private static async Task SeedCoreData(Exam_sysContext context, ILogger logger)
        {
            try
            {
                // Skip if database already has data
                if (await context.Departments.AnyAsync())
                {
                    logger.LogInformation("Database already contains data - skipping core data seeding");
                    return;
                }

                logger.LogInformation("Seeding core system data...");

                // Departments
                var departments = new[]
                {
                    new Department { name = "Computer Science", isActive = true },
                    new Department { name = "Information Systems", isActive = true }
                };
                await context.Departments.AddRangeAsync(departments);

                // Branches
                var branches = new[]
                {
                    new Branch { name = "Main Branch", isActive = true },
                    new Branch { name = "Smart Village", isActive = true }
                };
                await context.Branches.AddRangeAsync(branches);

                await context.SaveChangesAsync();

                // Roles
                var roles = new[]
                {
                    new Role { RoleTitle = "Admin", isActive = true },
                    new Role { RoleTitle = "Instructor", isActive = true },
                    new Role { RoleTitle = "Student", isActive = true }
                };
                await context.Roles.AddRangeAsync(roles);

                // Permissions
                var permissions = new[]
                {
                    new Permission { PerTitle = "ManageUsers", isActive = true },
                    new Permission { PerTitle = "ManageExams", isActive = true }
                };
                await context.Permissions.AddRangeAsync(permissions);

                await context.SaveChangesAsync();

                // Assign all permissions to Admin role
                var adminRole = await context.Roles.FirstAsync(r => r.RoleTitle == "Admin");
                adminRole.Pers.AddRange(await context.Permissions.ToListAsync());
                await context.SaveChangesAsync();

                logger.LogInformation("Core system data seeded successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to seed core system data");
                throw;
            }
        }

        private static async Task EnsureAdminAccount(Exam_sysContext context, IConfiguration config, ILogger logger)
        {
            try
            {
                var adminEmail = config["AdminUser:Email"];
                var adminPassword = config["AdminUser:Password"];

                if (string.IsNullOrWhiteSpace(adminEmail))
                    throw new ArgumentNullException("AdminUser:Email configuration is required");
                if (string.IsNullOrWhiteSpace(adminPassword))
                    throw new ArgumentNullException("AdminUser:Password configuration is required");

                logger.LogInformation($"Ensuring admin account exists: {adminEmail}");

                // Check if admin already exists
                if (await context.Users.AnyAsync(u => u.email == adminEmail))
                {
                    logger.LogInformation($"Admin account {adminEmail} already exists");
                    return;
                }

                // Get required references
                var csDept = await context.Departments.FirstAsync(d => d.name == "Computer Science");
                var mainBranch = await context.Branches.FirstAsync(b => b.name == "Main Branch");
                var adminRole = await context.Roles.FirstAsync(r => r.RoleTitle == "Admin");

                // Create admin user
                var admin = new User
                {
                    name = config["AdminUser:Name"] ?? "System Administrator",
                    email = adminEmail,
                    phone = config["AdminUser:Phone"] ?? "+201000000000",
                    dept_id = csDept.dept_id,
                    branch_id = mainBranch.branch_id,
                    st_city = config["AdminUser:City"] ?? "Cairo",
                    isActive = true
                };

                admin.SetPassword(adminPassword);
                admin.Roles.Add(adminRole);

                context.Users.Add(admin);
                await context.SaveChangesAsync();

                logger.LogInformation($"Admin account {adminEmail} created successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to ensure admin account exists");
                throw;
            }
        }
    }
}