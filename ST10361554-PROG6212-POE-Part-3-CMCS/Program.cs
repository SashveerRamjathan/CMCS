using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ST10361554_PROG6212_POE_Part_3_CMCS.Data;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;
using ST10361554_PROG6212_POE_Part_3_CMCS.Services;
namespace ST10361554_PROG6212_POE_Part_3_CMCS
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Add Database Context
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

            // Register SeedDatabase service
            builder.Services.AddScoped<SeedDatabase>();

            // Changed from IdentityUser to ApplicationUser
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultTokenProviders();

            builder.Services.AddControllersWithViews();

            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Seed roles and HR user
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // ensure the database is created
                var dbContext = services.GetRequiredService<ApplicationDbContext>();

                // Code Attribution:
                // Ensure the local db is created at startup
                // Ikechi Michael
                // 9 October 2024
                // https://mykeels.medium.com/ef-cores-migration-seeder-gotchas-bcefcaff35c9
                await dbContext.Database.EnsureCreatedAsync();

                // get the services required to create roles and users
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Seed roles
                await SeedRoles(services);

                // get the seed database service
                var seedDatabase = services.GetRequiredService<SeedDatabase>();

                // Seed HR user
                await seedDatabase.SeedHRUserAsync();

                // Seed the lecturer users
                await seedDatabase.SeedLecturerUsersAsync();

                // Seed the academic manager users
                await seedDatabase.SeedAcademicManagerUsersAsync();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            await app.RunAsync();
        }

        // Code Attribution:
        // How To Seed Users And Roles With Code In ASP.NET Core Identity
        // Rajesh Shirsagar
        // 9 October 2024
        // https://www.linkedin.com/pulse/overview-how-seed-users-roles-code-aspnet-core-rajesh-shirsagar/
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            try
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

                string[] roleNames = { "Lecturer", "Academic Manager", "HR" };

                foreach (var role in roleNames)
                {
                    var roleExists = await roleManager.RoleExistsAsync(role);

                    if (!roleExists)
                    {
                        var result = await roleManager.CreateAsync(new IdentityRole(role));

                        if (result.Succeeded)
                        {
                            logger.LogInformation($"Role '{role}' created successfully.");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                logger.LogError($"Error creating role '{role}': {error.Description}");
                            }
                        }
                    }
                    else
                    {
                        logger.LogInformation($"Role '{role}' already exists.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while trying to seed user roles: {ex.Message}");
                return;
            }
        }
    }
}
