using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using DotVVM.Framework.Routing;
using Microsoft.AspNetCore.Mvc;
using AIServices.APIs;

namespace ObjectsApp;

public class Startup
{

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Latest);
        services.AddDataProtection();
        services.AddAuthorization();
        services.AddWebEncoders();
        services.AddTransient(typeof(CustomVision));
        services.AddDotVVM<DotvvmStartup>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMvcWithDefaultRoute();

        // use DotVVM
        var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(env.ContentRootPath);
        dotvvmConfiguration.AssertConfigurationIsValid();
            
        // use static files
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(env.WebRootPath)
        });
    }
}