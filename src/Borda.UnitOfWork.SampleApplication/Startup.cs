using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Borda.UnitOfWork.SampleApplication
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddDbContext<ApplicationContext>(opt =>
            {
                opt.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddCap(options =>
            {
                options.UseEntityFramework<ApplicationContext>();
                options.UseRabbitMQ(opt =>
                {
                    opt.HostName = Configuration.GetValue<string>("EventBus:Host");
                    opt.Port = Configuration.GetValue<int>("EventBus:Port");
                    opt.UserName = Configuration.GetValue<string>("EventBus:Username");
                    opt.Password = Configuration.GetValue<string>("EventBus:Password");
                });
            });
            
            services.AddUnitOfWork<ApplicationContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            AutoMigrate(app);
        }
        
        private static void AutoMigrate(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<ApplicationContext>();

            context?.Database.Migrate();
        }
    }
}