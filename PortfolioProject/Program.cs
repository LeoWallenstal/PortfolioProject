using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataLayer.Data;
using DataLayer.Models;
using DataLayer;

namespace PortfolioProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseLazyLoadingProxies()
                .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddIdentity<User, IdentityRole>()
            .AddErrorDescriber<SwedishIdentityErrorDescriber>()
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddScoped<IMessagesService, MessagesService>();

            //!!!!!! Notis till James !!!!!!
            //Om du väljer att ha kvar den separata klassen, ändra namnet här också
            //Om du istället gör det direkt i controllern så kan du ta bort denna rad
            builder.Services.AddScoped<ExportPlaceholder, ExportPlaceholder>();

            builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomClaimsFactory>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Error/403";
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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            SeedData.SeedAsync(app.Services);
            app.Run();            
        }
    }
}
