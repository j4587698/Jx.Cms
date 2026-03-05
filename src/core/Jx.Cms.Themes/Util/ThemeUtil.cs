using DeviceDetectorNET.Parser.Device;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Common.Utils;
using Jx.Cms.DbContext.Entities.Settings;
using Jx.Cms.Plugin;
using Jx.Cms.Themes.PartManager;
using Jx.Cms.Themes.RazorCompiler;
using Jx.Toolbox.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Constants = Jx.Cms.Common.Utils.Constants;

namespace Jx.Cms.Themes.Util;

public static class ThemeUtil
{
    private const string DefaultPcThemeName = "Default";

    private const string DefaultMobileThemeName = "Mobile";

    /// <summary>
    ///     变更通知
    /// </summary>
    public static Action<ThemeConfig> ThemeModify;

    /// <summary>
    ///     PC主题，不切换主题与自适应主题同样使用此主题
    /// </summary>
    public static string PcThemeName { get; private set; }

    /// <summary>
    ///     手机版主题
    /// </summary>
    public static string MobileThemeName { get; private set; }

    /// <summary>
    ///     主题切换方式，默认为不切换
    /// </summary>
    public static ThemeChangeMode Mode { get; set; } = ThemeChangeMode.None;

    /// <summary>
    ///     手机版域名
    /// </summary>
    public static string MobileDomain { get; set; }

    /// <summary>
    ///     获取主题路径
    /// </summary>
    public static string GetThemeName()
    {
        var httpContext = ServicesExtension.GetRequiredService<IHttpContextAccessor>().HttpContext;
        switch (Mode)
        {
            case ThemeChangeMode.None:
            case ThemeChangeMode.Adaptive:
                return PcThemeName;
            case ThemeChangeMode.Auto:
                return IsMobileRequest(httpContext) ? MobileThemeName : PcThemeName;
            case ThemeChangeMode.Domain:
                return IsCurrentMobileDomain(httpContext) ? MobileThemeName : PcThemeName;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    ///     跳转指定路径
    /// </summary>
    /// <returns></returns>
    public static string Redirect()
    {
        if (Mode != ThemeChangeMode.Domain || MobileDomain.IsNullOrEmpty()) return null;

        var httpContext = ServicesExtension.GetRequiredService<IHttpContextAccessor>().HttpContext;
        if (httpContext == null) return null;
        if (IsCurrentMobileDomain(httpContext)) return null;
        if (!IsMobileRequest(httpContext)) return null;

        return
            $"{httpContext.Request.Scheme}://{MobileDomain}{httpContext.Request.PathBase}{httpContext.Request.Path}{httpContext.Request.QueryString}";
    }

    /// <summary>
    ///     初始化模板设置
    /// </summary>
    public static void InitThemePath()
    {
        if (Common.Utils.Util.IsInstalled)
        {
            Mode = SettingsEntity.GetValue(nameof(Mode))?.ToEnum<ThemeChangeMode>() ?? ThemeChangeMode.None;
            PcThemeName = SettingsEntity.GetValue(nameof(PcThemeName)) ?? DefaultPcThemeName;
            MobileThemeName = SettingsEntity.GetValue(nameof(MobileThemeName)) ?? DefaultMobileThemeName;
            MobileDomain = SettingsEntity.GetValue(nameof(MobileDomain)) ?? "";
        }
        else
        {
            Mode = ThemeChangeMode.None;
            PcThemeName = DefaultPcThemeName;
            MobileThemeName = DefaultMobileThemeName;
            MobileDomain = "";
        }

        var themes = GetAllThemes();
        var needSave = false;
        if (PcThemeName != DefaultPcThemeName)
        {
            var pc = themes.FirstOrDefault(x => x.ThemeName == PcThemeName);
            if (pc != null)
                SetTheme(pc);
            else
            {
                PcThemeName = DefaultPcThemeName;
                needSave = true;
            }
        }

        if (MobileThemeName != DefaultMobileThemeName)
        {
            var mobile = themes.FirstOrDefault(x => x.ThemeName == MobileThemeName);
            if (mobile != null)
                SetTheme(mobile);
            else
            {
                MobileThemeName = DefaultMobileThemeName;
                needSave = true;
            }
        }

        if (needSave)
        {
            SettingsEntity.SetValue(nameof(PcThemeName), PcThemeName);
            SettingsEntity.SetValue(nameof(MobileThemeName), MobileThemeName);
        }
    }

    public static void SetTheme(ThemeConfig themeConfig)
    {
        if (themeConfig == null) throw new ArgumentNullException(nameof(themeConfig));
        if (themeConfig.ThemeName.IsNullOrEmpty())
            throw new ArgumentException("主题名称不能为空", nameof(themeConfig));

        var oldThemeName = themeConfig.ThemeType switch
        {
            ThemeType.PcTheme => PcThemeName,
            ThemeType.MobileTheme => MobileThemeName,
            _ => PcThemeName
        };

        switch (themeConfig.ThemeType)
        {
            case ThemeType.PcTheme:
                PcThemeName = themeConfig.ThemeName;
                break;
            case ThemeType.MobileTheme:
                MobileThemeName = themeConfig.ThemeName;
                break;
            case ThemeType.AdaptiveTheme:
                PcThemeName = themeConfig.ThemeName;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(themeConfig.ThemeType), themeConfig.ThemeType, null);
        }

        var hasChanged = !string.Equals(oldThemeName, themeConfig.ThemeName, StringComparison.OrdinalIgnoreCase);
        var isDefaultTheme = string.Equals(themeConfig.ThemeName, DefaultPcThemeName, StringComparison.OrdinalIgnoreCase);
        var currentAssembly = RazorPlugin.GetAssemblyByThemeType(themeConfig.ThemeType);
        var needRuntimeRefresh = hasChanged || (!isDefaultTheme && currentAssembly == null) ||
                                 (isDefaultTheme && currentAssembly != null);

        if (!hasChanged && !needRuntimeRefresh) return;

        if (hasChanged)
        {
            SettingsEntity.SetValue(nameof(Mode), Mode.ToString());
            SettingsEntity.SetValue(nameof(PcThemeName), PcThemeName);
            SettingsEntity.SetValue(nameof(MobileThemeName), MobileThemeName);
        }

        var applicationPartManager =
            ServicesExtension.Services.GetSingletonInstanceOrNull<ApplicationPartManager>();

        if (needRuntimeRefresh)
        {
            if (!isDefaultTheme)
                RazorPlugin.LoadPlugin(themeConfig, applicationPartManager);
            else
                RazorPlugin.RemovePlugin(themeConfig, applicationPartManager);

            MyActionDescriptorChangeProvider.Instance.NotifyChanges();
            var viewCompilerProvider =
                ServicesExtension.GetRequiredService<IViewCompilerProvider>() as MyViewCompilerProvider;
            viewCompilerProvider?.Modify();

            ThemeModify?.Invoke(themeConfig);
        }
    }

    public static List<ThemeConfig> GetAllThemes()
    {
        var themeConfigs = new List<ThemeConfig>();
        themeConfigs.Add(new ThemeConfig
        {
            IsUsing = PcThemeName == "Default",
            Path = "",
            ScreenShot = "",
            ThemeDescription = "默认主题",
            ThemeDisplayName = "默认主题",
            ThemeName = "Default",
            ThemeType = ThemeType.PcTheme
        });
        var dirs = Directory.GetDirectories(Constants.ThemePath);
        foreach (var dir in dirs)
        {
            var configPath = Path.Combine(dir, "theme.json");
            if (File.Exists(configPath))
                try
                {
                    var themeConfig = JsonConvert.DeserializeObject<ThemeConfig>(File.ReadAllText(configPath));
                    if (themeConfig == null || themeConfig.ThemeName.IsNullOrEmpty()) continue;

                    themeConfig.Path = dir;
                    switch (themeConfig.ThemeType)
                    {
                        case ThemeType.PcTheme:
                            themeConfig.IsUsing = PcThemeName == themeConfig.ThemeName;
                            break;
                        case ThemeType.MobileTheme:
                            themeConfig.IsUsing = MobileThemeName == themeConfig.ThemeName;
                            break;
                        case ThemeType.AdaptiveTheme:
                            themeConfig.IsUsing = PcThemeName == themeConfig.ThemeName;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    themeConfigs.Add(themeConfig);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    //throw;
                }
        }

        return themeConfigs;
    }

    private static bool IsCurrentMobileDomain(HttpContext httpContext)
    {
        if (httpContext == null || MobileDomain.IsNullOrEmpty()) return false;

        var requestHost = httpContext.Request.Host.Value;
        if (requestHost.Equals(MobileDomain, StringComparison.OrdinalIgnoreCase)) return true;
        return httpContext.Request.Host.Host.Equals(MobileDomain, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsMobileRequest(HttpContext httpContext)
    {
        var userAgent = httpContext?.Request.Headers[HeaderNames.UserAgent];
        if (!userAgent.HasValue) return false;

        var device = new MobileParser();
        device.SetUserAgent(userAgent.Value);
        return device.Parse().Success;
    }
}
