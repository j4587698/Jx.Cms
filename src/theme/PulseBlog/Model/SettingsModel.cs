using System.ComponentModel;
using PulseBlog.Enum;

namespace PulseBlog.Model;

[Description("Pulse主题设置")]
public class SettingsModel
{
    [DisplayName("Logo图片路径")]
    public string LogoUrl { get; set; } = "";

    [DisplayName("Favicon路径")]
    public string FaviconUrl { get; set; } = "/PulseBlog/img/favicon.svg";

    [DisplayName("首页主标题")]
    public string HeroTitle { get; set; } = "记录技术与思考";

    [DisplayName("首页副标题")]
    public string HeroSubtitle { get; set; } = "清晰表达、持续积累、长期主义。";

    [DisplayName("主题强调色")]
    public string AccentColor { get; set; } = "#0A7F78";

    [DisplayName("侧边栏布局")]
    public SidebarLayoutEnum Sidebar { get; set; } = SidebarLayoutEnum.Right;

    [DisplayName("站点关键词")]
    public string Keywords { get; set; } = "";

    [DisplayName("站点描述")]
    public string Description { get; set; } = "";

    [DisplayName("SEO默认分享图URL")]
    public string DefaultSocialImageUrl { get; set; } = "";

    [DisplayName("SEO作者名")]
    public string SeoAuthor { get; set; } = "";

    [DisplayName("Twitter账号(可选)")]
    public string TwitterSite { get; set; } = "";

    [DisplayName("顶部公告")]
    public string Notice { get; set; } = "";

    [DisplayName("页脚信息")]
    public string FooterInfo { get; set; } = "";

    [DisplayName("页头额外代码")]
    public string HeaderExtendCode { get; set; } = "";

    [DisplayName("页脚额外代码")]
    public string FooterExtendCode { get; set; } = "";
}
