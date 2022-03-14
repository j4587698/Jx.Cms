namespace Jx.Cms.Common.Utils
{
    public class SettingsConstants
    {
        /// <summary>
        /// 系统类型
        /// </summary>
        public const string SystemType = "system";

        /// <summary>
        /// 标题的键
        /// </summary>
        public const string TitleKey = "title";

        /// <summary>
        /// 副标题键
        /// </summary>
        public const string SubTitleKey = "subtitle";

        /// <summary>
        /// Url键
        /// </summary>
        public const string UrlKey = "url";

        /// <summary>
        /// 版权键
        /// </summary>
        public const string CopyRightKey = "copyright";

        /// <summary>
        /// 备案信息的键
        /// </summary>
        public const string BeiAnKey = "beian";

        /// <summary>
        /// 每页显示数量的键
        /// </summary>
        public const string CountPerPageKey = "contperpage";

        public static string[] GetAllSystemKey()
        {
            return new[] { TitleKey, SubTitleKey, UrlKey, CopyRightKey, BeiAnKey, CountPerPageKey };
        }
        
    }
}