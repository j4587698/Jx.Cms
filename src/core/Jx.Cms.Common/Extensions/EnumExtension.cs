using System;

namespace Jx.Cms.Common.Extensions
{
    public static class EnumExtension
    {
        public static string GetKey(this System.Enum en)
        {
            return en.ToString();
        }

        public static T ToEnum<T>(this string enStr) where T : struct, System.Enum
        {
            return System.Enum.Parse<T>(enStr);
        }
    }
}