using LargeFileAPI.Configuration;
using LargeFileAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using tusdotnet;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;
using tusdotnet.Stores;

namespace LargeFileAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRazorPages();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LargeFileAPI", Version = "v1" });
            });

            services.Configure<AppSettings>(Configuration.GetSection("AppConfig"));
            services.AddScoped<FileSaveService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LargeFileAPI v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseTus(httpContext => new DefaultTusConfiguration()
            {
                // where to store files
                Store = new TusDiskStore(@$"{Configuration.GetSection("AppConfig").GetValue<string>("FileSavePath")}\tusfiles\"),
                // On what url should we listen for uploads?
                UrlPath = "/tus/files",
                MaxAllowedUploadSizeInBytes = null,
                MaxAllowedUploadSizeInBytesLong = null,
                Events = new Events
                {
                    OnFileCompleteAsync = async eventContext =>
                    {
                        var fileSaveService = httpContext.RequestServices.GetService<FileSaveService>();
                        await fileSaveService.SaveFileUsingTus(eventContext);
                    }
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
