﻿using System.IO;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
            FontCollection collection = new FontCollection();
            var family = collection.Install(Resource.GetResource("font.ttf"));
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
            image.Mutate(x => x.DrawText(options, str, family.CreateFont(fontsize), color, new PointF(0, height / 2)));
            MemoryStream ms = new MemoryStream();
            image.SaveAsPng(ms);
            ms.Position = 0;
            return ms;
        }
    }
}