using System.IO;

namespace Jx.Cms.Common.Utils
{
    public class Util
    {
        /// <summary>
        /// 是否已安装
        /// </summary>
        public static bool IsInstalled { get; set; }

        static Util()
        {
            IsInstalled = File.Exists("install.lock");
        }
    }
}