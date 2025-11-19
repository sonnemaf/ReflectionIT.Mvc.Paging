using ReflectionIT.Mvc.Paging;
using SampleApp.Models.Database;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace SampleApp; 
public class Startup {

    public Startup(IConfiguration configuration) {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {

        services.AddDbContext<NorthwindContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("Northwind")));

        services.AddControllersWithViews();

        // Register ViewComponent using an EmbeddedFileProvider & setting some options
        services.AddPaging(options => {
            options.ViewName = "Bootstrap5";
            options.PageParameterName = "pageindex";
            options.SortExpressionParameterName = "sort";
            options.DefaultNumberOfPagesToShow = 3;
            options.HtmlIndicatorDown = " <span>&darr;</span>";
            options.HtmlIndicatorUp = " <span>&uarr;</span>";
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime) {

        if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        } else {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => {

            endpoints.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
               );

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

    }

   
}
