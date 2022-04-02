using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Jx.Cms.Common.Utils
{
    public static class Resource
    {
        public static Stream GetResource(string resourceName)
        {
            return GetResource(typeof(Resource), resourceName);
        }

        public static Stream GetResource(Type assemblyType, string resourceName)
        {
            var name = assemblyType.Assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(resourceName));
            if (name != null)
            {
                return assemblyType.Assembly.GetManifestResourceStream(name);
            }

            return null;
        }
    }
}