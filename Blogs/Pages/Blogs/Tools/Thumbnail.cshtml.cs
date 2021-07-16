using System;
using System.IO;
using System.Threading.Tasks;
using Blogs.Utils;
using Furion.RemoteRequest.Extensions;
using Masuit.Tools.Media;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Blogs.Pages.Blogs.Tools
{
    public class Thumbnail : PageModel
    {
        public async Task<IActionResult> OnGet(string src, int width, int height, string op = "crop")
        {
            src = HttpContext.GetUrl(src);
            var image = await Image.LoadAsync(await src.GetAsStreamAsync());
            if (op == "crop")
            {
                image.Mutate(x => x.Crop(width, height));
            }
            else if (op == "resize")
            {
                image.Mutate( x => x.Resize(width, height));
            }
            var stream = new MemoryStream();
            await image.SaveAsync(stream, JpegFormat.Instance);
            stream.Position = 0;
            return File(stream, "image/jpeg");
        }
    }
}