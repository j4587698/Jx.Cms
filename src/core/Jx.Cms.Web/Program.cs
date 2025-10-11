using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jx.Cms.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Jx.Cms.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "config")))
            {
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "config"));
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStaticWebAssets();
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((context, configuration) =>
                    configuration.ReadFrom.Configuration(context.Configuration));
    }
}