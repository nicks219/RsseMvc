using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.DBContext;
using System;
using System.IO;
using RandomSongSearchEngine.Logger;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using RandomSongSearchEngine.Middleware;
using System.Runtime.CompilerServices;

//TODO: ÏÅÐÅÄ ÏÓÁËÈÊÀÖÈÅÉ ÍÅ ÇÀÁÓÄÜ ÓÁÐÀÒÜ ÏÎÄÊËÞ×ÅÍÈß Ê ÁÄ

namespace RandomSongSearchEngine
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
            //services.AddRazorPages();
            services.AddMvc(options => { options.EnableEndpointRouting = false; });
            services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //app.UseMiddleware<GCMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseRouting();
            app.UseAuthorization();
            app.UseMvcWithDefaultRoute();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //});

            loggerFactory.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));
            var logger = loggerFactory.CreateLogger("FileLogger");
            logger.LogInformation("Application started at {0}", DateTime.Now);
        }
    }
}
