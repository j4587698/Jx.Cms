using Jx.Cms.Install;
using Jx.Cms.Install.Pages;
using Jx.Cms.Web;
using Jx.Cms.Web.Components;
using Jx.Toolbox.Mvc.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo
    .File("./log/log.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
    .WriteTo.Console().CreateLogger();

if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "config")))
    Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "config"));

var builder = WebApplication.CreateBuilder(args).AddToolbox(configOption =>
{
    configOption.ConfigSearchFolder = ["config"];
    configOption.DynamicPrefix = "/api/";
});

// Configure Serilog
builder.Host.UseSerilog();

// Create and configure startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

// Add services for Blazor WebApp
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build().UseToolbox();

// Configure middleware
var env = app.Environment;
startup.Configure(app, env);
app.UseAntiforgery();
app.MapDefaultControllerRoute();
app.MapRazorComponents<App>().AddAdditionalAssemblies(typeof(Install).Assembly)
    .AddInteractiveServerRenderMode();
app.Run();