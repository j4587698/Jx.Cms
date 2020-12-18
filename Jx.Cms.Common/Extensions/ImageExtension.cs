using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Jx.Cms.Common.Extensions
{
    public static class ImageExtension
    {
        /// <summary>  
        ///  Resize图片   
        /// </summary>  
        /// <param name="bmp">原始Bitmap </param>  
        /// <param name="newWidth">新的宽度</param>  
        /// <param name="newHeight">新的高度</param>  
        /// <returns>处理以后的图片</returns>  
        public static Bitmap ResizeImage(this Bitmap bmp, int newWidth, int newHeight)
        {
            try
            {
                var b = new Bitmap(newWidth, newHeight);
                using var g = Graphics.FromImage(b);
                // 插值算法的质量   
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                return b;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}