using System;
using System.Collections.Generic;
using DeviceDetectorNET.Parser.Device;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Entities.Settings;
using Jx.Cms.Themes.Config;
using Microsoft.Net.Http.Headers;

namespace Jx.Cms.Themes.Util
{
    public static class Utils
    {
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
        /// DLL与路径对应关系
        /// </summary>
        public static readonly Dictionary<string, string> PathDllDic = new Dictionary<string, string>();
        
        /// <summary>
        /// 路径与主题对应关系
        /// </summary>
        public static readonly Dictionary<string, string> ThemePathDic = new Dictionary<string, string>();

        /// <summary>
        /// 变更通知
        /// </summary>
        public static Action<string> ThemeModify;

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
            Mode = SettingsEntity.GetValue(nameof(Mode))?.ToEnum<ThemeChangeMode>() ?? ThemeChangeMode.None;
            PcThemeName = SettingsEntity.GetValue(nameof(PcThemeName)) ?? "Default";
            MobileThemeName = SettingsEntity.GetValue(nameof(MobileThemeName)) ?? "Mobile";
            MobileDomain = SettingsEntity.GetValue(nameof(MobileDomain)) ?? "";
        }

        public static void SetTheme(string themeName, ThemeType themeMode)
        {
            var oldThemeName = themeMode switch
            {
                ThemeType.PcTheme => PcThemeName,
                ThemeType.MobileTheme => MobileThemeName,
                _ => PcThemeName
            };
            var needChange = GetThemeName() == oldThemeName;
            switch (themeMode)
            {
                case ThemeType.PcTheme:
                    PcThemeName = themeName;
                    break;
                case ThemeType.MobileTheme:
                    MobileThemeName = themeName;
                    break;
                case ThemeType.AdaptiveTheme:
                    PcThemeName = themeName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(themeMode), themeMode, null);
            }

            if (needChange)
            {
                ThemeModify?.Invoke(themeName);
            }
        }
    }
}