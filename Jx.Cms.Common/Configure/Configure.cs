using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jx.Cms.Common.Configure
{
    public static class Configure
    {
        public static IServiceCollection ServiceCollection;

        public static IServiceProvider ServiceProvider { get; set; }
        
        public static IConfiguration Configuration { get; private set; }

        public const string AppSettingsJsonPath = "config/appsettings.json";

        private static readonly IConfigurationBuilder Builder;

        static Configure()
        {
            Builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(AppSettingsJsonPath, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = Builder.Build();
        }

        public static void AddJsonFile(string path)
        {
            if (!File.Exists(path) || Path.GetExtension(path) != ".json") return;
            Builder.AddJsonFile(path);
            Configuration = Builder.Build();
        }
    }
}