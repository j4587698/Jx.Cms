using System.IO;

namespace Jx.Cms.Common.Utils
{
    public static class Constants
    {
        public static readonly string LibraryPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "Theme"));
    }
}