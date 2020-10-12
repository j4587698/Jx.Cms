using Microsoft.AspNetCore.Http;

namespace Jx.Cms.Themes
{
    public static class HttpContext2
    {
        private static IHttpContextAccessor _accessor;
        
        public static HttpContext Current => _accessor.HttpContext;

        internal static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }
}