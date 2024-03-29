using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorServer.Data;
using Fluxor;
using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Fluxor.Blazor.Web.PersistStore;
//using Fluxor.Blazor.Web.DBreezePersistStore;
using Fluxor.Blazor.Web.MemoryPersistStore;

namespace BlazorServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddStorage();

            services.AddSingleton<WeatherForecastService>();

            // using Fluxor.Blazor.Web.DBreezePersistStore;
            //services.AddFluxorPersistStorage(this.Configuration);

            // using Fluxor.Blazor.Web.MemoryPersistStore;
            services.AddFluxorPersistStorage();

	        var currentAssembly = typeof(Startup).Assembly;
	        services.AddFluxor(fluxorOptions => fluxorOptions
                .ScanAssemblies(currentAssembly)
                .UseReduxDevTools()
                .UsePersistStore(options => { })
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
