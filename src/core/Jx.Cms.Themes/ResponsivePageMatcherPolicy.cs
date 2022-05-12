using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jx.Cms.Common.Extensions;
using Jx.Cms.Themes.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;

namespace Jx.Cms.Themes
{
    public class ResponsivePageMatcherPolicy: MatcherPolicy, IEndpointComparerPolicy, IEndpointSelectorPolicy
    {
        public ResponsivePageMatcherPolicy() => Comparer = EndpointMetadataComparer<ThemeNameAttribute>.Default;
        
        public override int Order => 100000;
        public IComparer<Endpoint> Comparer { get; }
        public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
        {
            foreach (var endpoint in endpoints)
            {
                if (endpoint?.Metadata.GetMetadata<IThemeNameMetadata>() != null) return true;
            }
            return false;
        }

        public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
        {
            List<CandidateState> candidateStates = new List<CandidateState>();
            for (int i = 0; i < candidates.Count; i++)
            {
                candidateStates.Add(candidates[i]);
            }

            if (candidateStates.MaxBy(x => x.Score).Values!.Any(x => x.Value?.ToString()?.StartsWith("/Admin") == true))
            {
                return Task.CompletedTask;
            }
            var path = ThemeUtil.GetThemeName();
            if (path.IsNullOrEmpty())
            {
                return Task.CompletedTask;
            }
            for (var i = 0; i < candidates.Count; i++)
            {
                var endpoint = candidates[i].Endpoint;
                var metaData = endpoint.Metadata.GetMetadata<IThemeNameMetadata>();
                if (metaData?.ThemeName != path)
                {
                    candidates.SetValidity(i, false);
                }
            }
            return Task.CompletedTask;
        }
    }
}