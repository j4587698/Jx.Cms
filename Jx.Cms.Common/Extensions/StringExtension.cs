using System.Text.RegularExpressions;

namespace Jx.Cms.Common.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 驼峰转下划线
        /// </summary>
        /// <param name="str">原驼峰模式字符串</param>
        /// <returns>下划线分割的字符串</returns>
        public static string ConvertToUnderLine(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return "";
            }

            string strItemTarget = Regex.Replace("AtestBtestCtest", "([A-Z])", "_$1").ToLower();

            return strItemTarget.TrimStart('_');
        }
    }
}