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
        
        public bool IsValidRequest(HttpContext context)
        {
            var extension = System.IO.Path.GetExtension(context.Request.Path.Value);
            return !string.IsNullOrEmpty(extension);
        }

        public Task<IImageResolver?> GetAsync(HttpContext context)
        {
            // For now, we'll just return null to avoid compatibility issues
            return Task.FromResult<IImageResolver?>(null);
        }

        public ProcessingBehavior ProcessingBehavior { get; } = ProcessingBehavior.CommandOnly;
        public Func<HttpContext, bool> Match { get; set; } = _ => true;
    }
}