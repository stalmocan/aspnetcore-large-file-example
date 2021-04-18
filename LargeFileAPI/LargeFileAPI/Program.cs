using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LargeFileAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(config =>
                    {
                        config.Limits.MaxRequestBodySize = int.MaxValue;
                        config.Limits.MaxRequestHeadersTotalSize = int.MaxValue;
                    });
                    
                });
    }
}
