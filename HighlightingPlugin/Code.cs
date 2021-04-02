using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HighlightingPlugin
{
    public enum Code
    {
        [Description("自动")]
        Auto,
        [Description("C#")]
        CSharp,
        [Description("JAVA")]
        Java,
        [Description("Markdown")]
        Markdown,
        [Description("PHP")]
        Php,
        [Description("C++")]
        Cpp,
        [Description("GO")]
        Go,
        [Description("JSON")]
        Json,
        [Description("SQL")]
        Sql,
        [Description("Python")]
        Python,
        [Description("Rust")]
        Rust,
        [Description("VB.NET")]
        VbNet,
        [Description("TypeScript")]
        TypeScript,
        [Description("Shell")]
        Sell,
        [Description("Lua")]
        Lua,
        [Description("Properties配置文件")]
        Properties,
        [Description("ini配置文件")]
        Ini,
        [Description("Html")]
        Html,
        [Description("CSS")]
        Css,
        [Description("Object-c")]
        Objc,
        [Description("Yaml")]
        Yaml,
        [Description("Perl")]
        Perl
    }
}