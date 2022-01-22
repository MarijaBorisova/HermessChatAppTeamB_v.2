using HermessChatAppTeamB_v._2.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HermessChatAppTeamB_v._2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IUserValidator<User>, CustomUserValidator>();

             services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));//to add services for EntityFrameworkCore
             
             services.AddIdentity<User, IdentityRole>(opts => {
                 opts.User.RequireUniqueEmail = true;    // unique email
                 opts.User.AllowedUserNameCharacters = ".@abcdefghijklmnopqrstuvwxyz"; // appropriate symbols
             })
                   .AddEntityFrameworkStores<ApplicationContext>()
                   .AddDefaultTokenProviders() ;//to add services of Identity and the storage of saving Identity data
            
             services.AddRazorPages();

             services.AddControllersWithViews();
            /*string conStr = "Server=(localdb)\\mssqllocaldb;Database=usersdb46;Trusted_Connection=True;MultipleActiveResultSets=true";
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(conStr));*/
          
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();    // con to authentication, add the component for usage of Identity/middleware
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
      
        }
    }
