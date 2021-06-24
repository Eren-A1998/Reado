using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OBS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OBS.Models;
using OBS.Services;
using Stripe;
using Microsoft.Extensions.Options;

namespace OBS
{
    public class Startup
    {
       // private readonly RoleManager<IdentityRole> roleManager;

        //private async Task CreateRoles()
        //{
        //    ApplicationDbContext context = new ApplicationDbContext(DbContextOptions < ApplicationDbContext > Options);


        //    //initializing custom roles 
        //    var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //    var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        //    string[] roleNames = { "Admin", "Manager", "Member" };

        //    IdentityResult roleResult;

        //    foreach (var roleName in roleNames)
        //    {
        //        var roleExist = await RoleManager.RoleExistsAsync(roleName);
        //        if (!roleExist)
        //        {
        //            //create the roles and seed them to the database: Question 1
        //            roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
        //        }
        //    }

        //    //Here you could create a super user who will maintain the web app
        //    var poweruser = new ApplicationUser();
        //    //{

        //    //UserName = "ereny" ,              //Configuration["AppSettings:UserName"],
        //    //Email ="rona@gmail.com"          //Configuration["AppSettings:UserEmail"],
        //    //};

        //    poweruser.UserName = "ereny";
        //    poweruser.Email = "rona@gmail.com";

        //    //Ensure you have these values in your appsettings.json file
        //    string userPWD = "123";                     //Configuration["AppSettings:UserPassword"];
        //                                                //var _user = await UserManager.FindByEmailAsync();      //Configuration["AppSettings:AdminUserEmail"]

        //    //if (_user == null)
        //    //{
        //    var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
        //    if (createPowerUser.Succeeded)
        //    {
        //        //here we tie the new user to the role
        //        await UserManager.AddToRoleAsync(poweruser, "Admin");

        //    }
        //    //}
        //}
      //  var res = await roleManager.CreateAsync(new IdentityRole("Admin"));
        public  Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //CreateRoles(isv).Wait();

      


        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<ApplicationDbContext>(x => new ApplicationDbContext(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection"));
            //var serviceProvider = services.BuildServiceProvider();
            //services.AddScoped<IServiceProvider, ServiceProvider>();
            //services.AddIdentity<ApplicationUser, IdentityRole>();
            services.AddScoped<IUserBookService, UserBookService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IMyBookService, MyBookService>();

            services.AddDbContext<BooksContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("BooksCon")), ServiceLifetime.Transient
            );
            // services.AddAuthentication();

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                IConfigurationSection fbAuthNSection =
                    Configuration.GetSection("Facebook");
                facebookOptions.AppId = fbAuthNSection["AppId"];
                facebookOptions.AppSecret = fbAuthNSection["AppSecret"];
            }).AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection =
                    Configuration.GetSection("Google");

                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
            });
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            StripeConfiguration.SetApiKey(Configuration.GetSection("Stripe")["SecretKey"]);
            if (env.IsDevelopment())
            {
            
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();


            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
