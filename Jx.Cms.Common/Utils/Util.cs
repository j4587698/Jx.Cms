using System.Collections.Generic;
using System.IO;
using System.Linq;
using Furion;
using Jx.Cms.Common.Vo;
using Masuit.Tools.Security;
using Microsoft.Extensions.FileProviders;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Jx.Cms.Common.Utils
{
    public static class Util
    {
        /// <summary>
        /// 是否已安装
        /// </summary>
        public static bool IsInstalled { get; set; }

        /// <summary>
        /// 主题使用的文件提供器
        /// </summary>
        public static IFileProvider ThemeProvider { get; set; }

        private static readonly FontFamily Family;

        static Util()
        {
            IsInstalled = File.Exists("install.lock");
            FontCollection collection = new FontCollection();
            Family = collection.Install(Resource.GetResource("font.ttf"));
        }

        /// <summary>
        /// 将字符串转换为图片
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="width">图片长</param>
        /// <param name="height">图片宽</param>
        /// <param name="fontsize">图片的字号</param>
        /// <param name="color">文字颜色</param>
        /// <param name="bgColor">背景色</param>
        /// <returns>图片Stream</returns>
        public static Stream StringToImage(string str, int width, int height, int fontsize, Color color, Color bgColor)
        {
            Image image = new Image<Rgba32>(width, height, bgColor);
            var options = new DrawingOptions()
            {
                TextOptions = new TextOptions()
                {
                    ApplyKerning = true,
                    TabWidth = 8, // a tab renders as 8 spaces wide
                    WrapTextWidth = width, // greater than zero so we will word wrap at 100 pixels wide
                    HorizontalAlignment = HorizontalAlignment.Center, // right align
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            image.Mutate(x => x.DrawText(options, str, Family.CreateFont(fontsize), color, new PointF(0, height / 2)));
            MemoryStream ms = new MemoryStream();
            image.SaveAsPng(ms);
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// 获取Avatar头像地址
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GetWebAvatarUrl(string email)
        {
            return $"https://cn.gravatar.com/avatar/{email.ToLower().MDString()}";
        }

        /// <summary>
        /// 获取上传文件
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static List<MediaInfoVo> GetUploadFile(params string[] ext)
        {
            var uploadPath = Path.Combine(App.WebHostEnvironment.WebRootPath, "upload");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            return Directory.GetFiles(uploadPath, "*.*", SearchOption.AllDirectories)
                .Where(x => ext.Contains(Path.GetExtension(x))).Select(x => new MediaInfoVo()
                    { MediaName = Path.GetFileNameWithoutExtension(x), MediaInfo = new FileInfo(x), FullPath = x }).ToList();
        }

        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="inputStream">输入流</param>
        /// <param name="width">图片宽度</param>
        /// <param name="height">图片高度</param>
        /// <param name="op">操作码(crop:裁切 resize:重置大小)</param>
        /// <returns></returns>
        public static Stream GetThumbnail(Stream inputStream, int width, int height, string op = "crop")
        {
            var image = Image.Load(inputStream);
            if (op == "crop")
            {
                image.Mutate(x => x.Crop(width, height));
            }
            else if (op == "resize")
            {
                image.Mutate( x => x.Resize(width, height));
            }
            var stream = new MemoryStream();
            image.Save(stream, JpegFormat.Instance);
            stream.Position = 0;
            return stream;
        }
    }
}