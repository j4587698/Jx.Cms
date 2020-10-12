using System;

namespace Jx.Cms.Themes
{
    public class ThemeNameAttribute: Attribute, IThemeNameMetadata
    {
        public ThemeNameAttribute(string themeName) => ThemeName = themeName;
        
        public string ThemeName { get; }
    }
    
    public interface IThemeNameMetadata
    {
        string ThemeName { get; }
    }
}