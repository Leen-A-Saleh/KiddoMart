using All_Baby_Essentials.Areas.Admin.Mappings;

using All_Baby_Essentials.Data;
using All_Baby_Essentials.Models;
using All_Baby_Essentials.Services;
using All_Baby_Essentials.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace All_Baby_Essentials
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // identity
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            // services
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();
            
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

            var app = builder.Build();

            // saad data

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string[] roleNames = { "Admin", "User" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // admin initialization
                const string adminEmail = "leenanas412@gmail.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    var newAdmin = new ApplicationUser
                    {
                        FullName = "Leen Anas Saleh",
                        DateOfBirth = new DateOnly(2001, 4, 9),
                        UserName = adminEmail,
                        Email = adminEmail,
                        Gender = ApplicationUserGender.F,
                        EmailConfirmed = true,
                        PhoneNumber = "0799999999"
                    };

                    var result = await userManager.CreateAsync(newAdmin, "Leenanas412@");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdmin, "Admin");
                    }
                }
                else if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                var allUsers = await userManager.Users.ToListAsync();
                foreach (var user in allUsers)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    if (!roles.Any())
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }

                // Seed standard colors
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                if (!await context.Colors.AnyAsync())
                {
                    context.Colors.AddRange(
                        new Color { Name = "Red", HexCode = "#FF0000" },
                        new Color { Name = "Blue", HexCode = "#0000FF" },
                        new Color { Name = "Pink", HexCode = "#FFC0CB" },
                        new Color { Name = "White", HexCode = "#FFFFFF" },
                        new Color { Name = "Black", HexCode = "#000000" },
                        new Color { Name = "Yellow", HexCode = "#FFFF00" },
                        new Color { Name = "Green", HexCode = "#008000" },
                        new Color { Name = "Purple", HexCode = "#800080" },
                        new Color { Name = "Beige", HexCode = "#F5F5DC" },
                        new Color { Name = "Gray", HexCode = "#808080" }
                    );
                    await context.SaveChangesAsync();
                }
            }

            // middlware 
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // routing
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"
            );

            app.MapRazorPages();

            app.Run();
        }
    }
}








