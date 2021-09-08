using System;
using System.Threading.Tasks;
using Jx.Cms.Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.FileProviders;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;

namespace Jx.Cms.Common.Provider
{
    public class ThemeProvider: IImageProvider
    {
        

        /// <summary>
        /// Contains various format helper methods based on the current configuration.
        /// </summary>
        private readonly FormatUtilities _formatUtilities;

        public ThemeProvider(FormatUtilities formatUtilities)
        {
            _formatUtilities = formatUtilities;
        }
        
        public bool IsValidRequest(HttpContext context) => _formatUtilities.GetExtensionFromUri(context.Request.GetDisplayUrl()) != null;

        public Task<IImageResolver> GetAsync(HttpContext context)
        {
            // Path has already been correctly parsed before here.
            IFileInfo fileInfo = Util.ThemeProvider.GetFileInfo(context.Request.Path.Value);

            // Check to see if the file exists.
            if (!fileInfo.Exists)
            {
                return Task.FromResult<IImageResolver>(null);
            }

            var metadata = new ImageMetadata(fileInfo.LastModified.UtcDateTime, fileInfo.Length);
            return Task.FromResult<IImageResolver>(new PhysicalFileSystemResolver(fileInfo, metadata));
        }

        public ProcessingBehavior ProcessingBehavior { get; } = ProcessingBehavior.All;
        public Func<HttpContext, bool> Match { get; set; } = _ => true;
    }
}