using Microsoft.AspNetCore.OutputCaching;

namespace Movies.Api.Minimal.OutputCache
{
    public class ClaimAwareCachePolicy : IOutputCachePolicy
    {
        private readonly string _claimType;

        public ClaimAwareCachePolicy(string claimType)
        {
            _claimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
        }

        public async ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
        {
            var user = context.HttpContext.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                var claimValue = user.FindFirst(_claimType)?.Value ?? "no-claim";
                context.CacheVaryByRules.VaryByValues[$"claim:{_claimType}"] = claimValue;
            }
            else
            {
                context.CacheVaryByRules.VaryByValues["user"] = "anonymous";
            }
            context.CacheVaryByRules.RouteValueNames = new[] { "idOrSlug" };

            context.Tags.Add("movie-get-one");
            context.EnableOutputCaching = true;
            await Task.CompletedTask;
        }

        public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation)
        {
            return new ValueTask(Task.CompletedTask);
        }

        public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation)
        {
            return ValueTask.CompletedTask;
        }
    }
}
