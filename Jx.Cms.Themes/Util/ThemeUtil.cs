using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeviceDetectorNET.Parser.Device;
using Furion;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Common.Utils;
using Jx.Cms.Entities.Settings;
using Jx.Cms.Plugin;
using Jx.Cms.Themes.PartManager;
using Jx.Cms.Themes.RazorCompiler;
using Masuit.Tools.Core.Net;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Serilog.Core;
using Constants = Jx.Cms.Common.Utils.Constants;

namespace Jx.Cms.Themes.Util
{
    public static class ThemeUtil
    {
        private const string DefaultPcThemeName = "Default";

        private const string DefaultMobileThemeName = "Mobile";
        
        /// <summary>
        /// PC主题，不切换主题与自适应主题同样使用此主题
        /// </summary>
        public static string PcThemeName { get; private set; }

        /// <summary>
        /// 手机版主题
        /// </summary>
        public static string MobileThemeName { get; private set; } 

        /// <summary>
        /// 主题切换方式，默认为不切换
        /// </summary>
        public static ThemeChangeMode Mode { get; set; } = ThemeChangeMode.None;

        /// <summary>
        /// 手机版域名
        /// </summary>
        public static string MobileDomain { get; set; }

        /// <summary>
        /// 变更通知
        /// </summary>
        public static Action<ThemeConfig> ThemeModify;

        /// <summary>
        /// 获取主题路径
        /// </summary>
        public static string GetThemeName()
        {
            switch (Mode)
            {
                case ThemeChangeMode.None:
                case ThemeChangeMode.Adaptive:
                    return PcThemeName;
                case ThemeChangeMode.Auto:
                    var userAgent = HttpContext2.Current?.Request.Headers[HeaderNames.UserAgent];
                    if (!userAgent.HasValue)
                    {
                        return PcThemeName;
                    }
                    var device = new MobileParser();
                    device.SetUserAgent(userAgent.Value);
                    if (device.Parse().Success)
                    {
                        return MobileThemeName;
                    }

                    return PcThemeName;
                case ThemeChangeMode.Domain:
                    return MobileDomain;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 跳转指定路径
        /// </summary>
        /// <returns></returns>
        public static string Redirect()
        {
            if (MobileDomain.IsNullOrEmpty())
            {
                return null;
            }
            
            var userAgent = HttpContext2.Current?.Request.Headers[HeaderNames.UserAgent];
            if (!userAgent.HasValue)
            {
                return null;
            }
            
            if (HttpContext2.Current?.Request.Host.Value != MobileDomain)
            {
                var url = $"{HttpContext2.Current.Request.Scheme}://{MobileDomain}{HttpContext2.Current.Request.QueryString}";
                return url;
            }

            return null;
        }

        /// <summary>
        /// 初始化模板设置
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
            if (PcThemeName != DefaultPcThemeName)
            {
                var pc = themes.FirstOrDefault(x => x.ThemeName == PcThemeName);
                SetTheme(pc);
            }

            if (MobileThemeName != DefaultMobileThemeName)
            {
                var mobile = themes.FirstOrDefault(x => x.ThemeName == MobileThemeName);
                SetTheme(mobile);
            }
        }

        public static void SetTheme(ThemeConfig themeConfig)
        {
            var oldThemeName = themeConfig.ThemeType switch
            {
                ThemeType.PcTheme => PcThemeName,
                ThemeType.MobileTheme => MobileThemeName,
                _ => PcThemeName
            };
            var needChange = GetThemeName() == oldThemeName;
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

            if (needChange)
            {
                SettingsEntity.SetValue(nameof(Mode), Mode.ToString());
                SettingsEntity.SetValue(nameof(PcThemeName), PcThemeName);
                SettingsEntity.SetValue(nameof(MobileThemeName), MobileThemeName);
                if (themeConfig.ThemeName != "Default")
                {
                    var applicationPartManager = ServicesExtension.Services.GetSingletonInstanceOrNull<ApplicationPartManager>();
                    RazorPlugin.LoadPlugin(themeConfig, applicationPartManager);
                    MyActionDescriptorChangeProvider.Instance.HasChanged = true;
                    MyActionDescriptorChangeProvider.Instance.TokenSource.Cancel();
                    var viewCompilerProvider = App.GetService<IViewCompilerProvider>() as MyViewCompilerProvider;
                    viewCompilerProvider?.Modify();
                }
                else
                {
                    var applicationPartManager = ServicesExtension.Services.GetSingletonInstanceOrNull<ApplicationPartManager>();
                    RazorPlugin.RemovePlugin(themeConfig, applicationPartManager);
                }
                ThemeModify?.Invoke(themeConfig);
            }
        }

        public static List<ThemeConfig> GetAllThemes()
        {
            var themeConfigs = new List<ThemeConfig>();
            themeConfigs.Add(new ThemeConfig()
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
                {
                    try
                    {
                        var themeConfig = JsonConvert.DeserializeObject<ThemeConfig>(File.ReadAllText(configPath));
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
            }

            return themeConfigs;
        }
    }
}