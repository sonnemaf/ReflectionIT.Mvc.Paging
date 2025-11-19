using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReflectionIT.Mvc.Paging;
using SampleApp.Models.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<NorthwindContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("Northwind")));

// Register ViewComponent using an EmbeddedFileProvider & setting some options
builder.Services.AddPaging(options => {
    options.ViewName = "Bootstrap5";
    options.PageParameterName = "pageindex";
    options.SortExpressionParameterName = "sort";
    options.DefaultNumberOfPagesToShow = 3;
    options.HtmlIndicatorDown = " <span>&darr;</span>";
    options.HtmlIndicatorUp = " <span>&uarr;</span>";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

#if NET8_0
        app.UseStaticFiles();
#else
app.MapStaticAssets();
#endif

app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    )
#if NET8_0
    ;
#else
    .WithStaticAssets();
#endif

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
#if NET8_0
    ;
#else
    .WithStaticAssets();
#endif

app.Run();
