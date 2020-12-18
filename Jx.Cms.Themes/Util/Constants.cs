using System.IO;

namespace Jx.Cms.Themes.Util
{
    public static class Constants
    {
        public static readonly string LibraryPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "Theme"));
    }
}