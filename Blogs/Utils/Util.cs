using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Furion.RemoteRequest.Extensions;
using Jx.Cms.DbContext.Entities.Article;
using Masuit.Tools.Html;
using Microsoft.AspNetCore.Http;

namespace Blogs.Utils
{
    public static class Util
    {
        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="articleEntity"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string GetThumbnail(this ArticleEntity articleEntity, int width, int height)
        {
            string imgSrc = "";
            var img = GetHtmlImageUrlList(articleEntity.Content);
            if (img.Length > 0)
            {
                imgSrc = $@"{img[0]}?width={width}&height={height}'>";
            }
            else
            {
                Random random = new Random();
                imgSrc = $@"<img src='/Blogs/Image/random/{random.Next(10) + 1}.jpg?width={width}&height={height}&rmode=crop'>";
            }

            return imgSrc;
        }
        
        /// <summary> 
        /// 取得HTML中所有图片的 URL。 
        /// </summary> 
        /// <param name="sHtmlText">HTML代码</param> 
        /// <returns>图片的URL列表</returns> 
        public static string[] GetHtmlImageUrlList(string sHtmlText)
        {
            // 定义正则表达式用来匹配 img 标签 
            Regex regImg =
                new Regex(
                    @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>",
                    RegexOptions.IgnoreCase | RegexOptions.Multiline);

            // 搜索匹配的字符串 
            MatchCollection matches = regImg.Matches(sHtmlText);
            int i = 0;
            string[] sUrlList = new string[matches.Count];

            // 取得匹配项列表 
            foreach (Match match in matches)
                sUrlList[i++] = match.Groups["imgUrl"].Value;
            return sUrlList;
        }

        /// <summary>
        /// 根据HttpContext获取URL
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetUrl(this HttpContext httpContext, string url)
        {
            if (url.StartsWith("http"))
            {
                return url;
            }

            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{(url.StartsWith("/")?"":"/")}{url}";
        }

        /// <summary>
        /// 回复内容添加表情
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string BiaoQing(string content)
		{
            Regex regex = new Regex(@"(\[face_[0-3][0-9]{0,1}\])");
            return regex.Replace(content, @"<img src='/Blogs/Image/smilies/\$1.gif' alt='$1' class='wp-smiley'>");
		}
    }
}