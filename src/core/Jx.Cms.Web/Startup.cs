using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using BootstrapBlazor.Components;
using FreeSql;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Common.Utils;
using Jx.Cms.DbContext;
using Jx.Cms.Install.Middlewares;
using Jx.Cms.Plugin.Cache;
using Jx.Cms.Plugin.Middlewares;
using Jx.Cms.Plugin.Options;
using Jx.Cms.Themes;
using Jx.Cms.Themes.Middlewares;
using Jx.Cms.Themes.Options;
using Jx.Cms.Themes.PartManager;
using Jx.Cms.Themes.RazorCompiler;
using Jx.Toolbox.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers;

namespace Jx.Cms.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 1. PluginStartup services (int.MaxValue - 100)
            services.ConfigureOptions<Jx.Cms.Plugin.Options.UiConfigureOptions>();
            if (Util.IsInstalled)
            {
                WidgetCache.UpdateCache();
            }

            // 2. DbStartup services (int.MaxValue - 50)
            services.AddConfigurableOptions<DbConfig>(Configuration);
            services.AddTransient<DbStartup>();
            var dbConfig = Configuration.GetSection("Db").Get<DbConfig>();
            if (dbConfig != null)
            {
                var ret = SetupDb(services, dbConfig);
                if (!ret.isSuccess)
                {
                    throw new Common.Exceptions.DbException(ret.msg);
                }
                services.Configure<DbConfig>(x => x.CopyFrom(dbConfig));
            }
            else if (Util.IsInstalled)
            {
                throw new Common.Exceptions.DbException("数据库配置错误，无数据库配置信息！");
            }

            // 3. InstallStartup services (int.MaxValue - 20)
            // No services to register

            // 4. CommonStartup services (int.MaxValue)
            ServicesExtension.Services = services;
            services.AddImageSharp().RemoveProvider<PhysicalFileSystemProvider>().AddProvider<Jx.Cms.Common.Provider.ThemeProvider>();

            // 5. ThemeStartup services (default)
            string themePath = Path.Combine(Directory.GetCurrentDirectory(), "Theme");
            if (!Directory.Exists(themePath))
            {
                Directory.CreateDirectory(themePath);
            }
            
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Clear();
                options.AreaViewLocationFormats.Add("/{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/{2}/Views/Shared/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
                options.ViewLocationExpanders.Add(new Jx.Cms.Themes.TemplateViewLocationExpander());
            });

            services.AddRazorPages(options =>
            {
                options.Conventions.Add(new Jx.Cms.Themes.ResponsivePageRouteModelConvention());
            });

            services.AddSingleton<MatcherPolicy, Jx.Cms.Themes.ResponsivePageMatcherPolicy>();
            services.AddSingleton<IActionDescriptorChangeProvider>(MyActionDescriptorChangeProvider.Instance);
            services.AddSingleton(MyActionDescriptorChangeProvider.Instance);
            services.Replace<IViewCompilerProvider, MyViewCompilerProvider>();
            services.ConfigureOptions<Jx.Cms.Themes.Options.UiConfigureOptions>();

            // Original services
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();
            services.AddHttpClient();
            services.AddScoped<HttpClient>();
            services.Configure<CookiePolicyOptions>(op =>
            {
                op.CheckConsentNeeded = context => true;
                op.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(op =>
            {
                op.LoginPath = "/Admin/Login";
            });
            services.AddServerSideBlazor();
            services.AddBootstrapBlazor();
            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
                o.MaximumReceiveMessageSize = Int64.MaxValue;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 设置服务提供器
            ServicesExtension.ServiceProvider = app.ApplicationServices;
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 1. PluginStartup middleware
            app.UseMiddleware<PluginMiddleware>();

            // 2. ThemeStartup middleware
            app.UseMiddleware<RedirectMiddleware>();
            app.UseMiddleware<RewriteMiddleware>();

            // 3. CommonStartup middleware
            app.UseImageSharp();
            app.UseStaticFiles();

            // 4. InstallStartup middleware
            app.UseMiddleware<InstallMiddleware>("/Install/Step1");

            // Original middleware
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            
            // Blazor and common endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
                endpoints.MapFallbackToPage("~/Install/{*clientroutes:nonfile}", "/_InstallHost");
            });

            // Admin endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute("admin", "Admin", "/Admin/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapBlazorHub("~/_blazor");
                endpoints.MapFallbackToPage("~/Admin/{*clientroutes:nonfile}","/Admin/_AdminHost");
            });
        }

        // DbStartup methods
        public static bool CreateTables(DbConfig dbConfig)
        {
            try
            {
                var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x =>
                    x.GetTypes().Where(y => y.BaseType != null && y.BaseType.IsGenericType && y.BaseType.GetGenericTypeDefinition() == typeof(BaseEntity<,>)
                                            && y.FullName != null && !y.FullName.Contains("FreeSql")));
                BaseEntity.Orm.CodeFirst.SyncStructure(types.ToArray());
                var filePath = Path.Combine(AppContext.BaseDirectory, "config", "dbsettings.json");
                var jObject = File.Exists(filePath) ? JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filePath)) : new JObject();
                jObject["Db"] = JObject.Parse(JsonConvert.SerializeObject(dbConfig));
                File.WriteAllText(filePath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
            }
            catch (Exception e)
            {
                return false;
            }
            
            return true;
        }

        public static (bool isSuccess, string msg) SetupDb(IServiceCollection services, DbConfig dbConfig)
        {
            if (!dbConfig.DbType.IsNullOrEmpty() && Enum.TryParse(dbConfig.DbType, true, out DataType dataType))
            {
                var isDevelopment = true;
                string connStr = "";
                switch (dataType)
                {
                    case DataType.MySql:
                        connStr = $"Data Source={dbConfig.DbUrl};Port={dbConfig.DbPort};User ID={dbConfig.Username};Password={dbConfig.Password}; Initial Catalog={dbConfig.DbName};Charset=utf8; SslMode=none;Min pool size=1";
                        break;
                    case DataType.SqlServer:
                        connStr = $"Data Source={dbConfig.DbUrl},{dbConfig.DbPort};User Id={dbConfig.Username};Password={dbConfig.Password};Initial Catalog={dbConfig.DbName};TrustServerCertificate=true;Pooling=true;Min Pool Size=1";
                        break;
                    case DataType.PostgreSQL:
                        connStr = $"Host={dbConfig.DbUrl};Port={dbConfig.DbPort};Username={dbConfig.Username};Password={dbConfig.Password}; Database={dbConfig.DbName};Pooling=true;Minimum Pool Size=1";
                        break;
                    case DataType.Oracle:
                        connStr = $"user id={dbConfig.Username};password={dbConfig.Password}; data source=//{dbConfig.DbUrl}:{dbConfig.DbPort}/{dbConfig.DbName};Pooling=true;Min Pool Size=1";
                        break;
                    case DataType.Sqlite:
                        var path = Path.GetDirectoryName(dbConfig.DbName);
                        if (!path.IsNullOrEmpty() && !Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        connStr = $"data source={(dbConfig.DbName.EndsWith(".db")?dbConfig.DbName : dbConfig.DbName + ".db")}";
                        break;
                    default:
                        return (false, "数据库类型不在指定范围内");
                }
                var freeSql = new FreeSqlBuilder()
                    .UseAutoSyncStructure(isDevelopment)
                    .UseNoneCommandParameter(true)
                    .UseConnectionString(dataType, connStr)
                    .Build();
                
                if (freeSql == null)
                {
                    return (false, "数据库初始化失败");
                }

                if (!freeSql.Ado.ExecuteConnectTest())
                {
                    freeSql.Dispose();
                    return (false, "数据库连接失败");
                }

                freeSql.Aop.ConfigEntity += (s, e) =>
                {
                    e.ModifyResult.Name = dbConfig.Prefix + e.EntityType.Name.Replace("Entity", "").ToUnderLine();
                };

                services.AddSingleton(freeSql);

                BaseEntity.Initialization(freeSql, null);

                return (true, "");
            }
            return (false, "数据库类型不在指定范围内");
        }
    }
}