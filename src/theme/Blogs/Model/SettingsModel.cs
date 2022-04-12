using System.ComponentModel;
using Blogs.Enum;

namespace Blogs.Model
{
    [Description("设置类")]
    public class SettingsModel
    {
        [DisplayName("Logo路径")] public string LogoUrl { get; set; } = "/Blogs/Image/logo1.png";
        
        [DisplayName("Favicon路径")] public string FaviconUrl { get; set; } = "/Blogs/Image/favicon1.ico";
        
        [DisplayName("颜色风格")] public string Color { get; set; } = "";

        [DisplayName("博客布局")] public BlogLayoutEnum Layout { get; set; } = BlogLayoutEnum.Left;

        [DisplayName("侧边栏布局")] public SidebarEnum Sidebar { get; set; } = SidebarEnum.Right;

        [DisplayName("连接符")]public string Connector { get; set; } = "-";

        [DisplayName("站点关键词")] public string Keywords { get; set; } = "";

        [DisplayName("站点描述")] public string Description { get; set; } = "";

        [DisplayName("首页公告栏")] public string Notice { get; set; } = "";

        [DisplayName("欢迎语")] public string WelcomeMessage { get; set; } = "";

        [DisplayName("右上角菜单")] public string RightMenu { get; set; } = "";

        [DisplayName("页脚站点信息")] public string FootInfo { get; set; } = "";

        [DisplayName("页头额外代码")] public string HeaderExtendCode { get; set; } = "";

        [DisplayName("页脚额外代码")] public string FooterExtendCode { get; set; } = "";
    }
}