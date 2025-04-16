using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Movies.Api.Auth
{
    public class MultiAuthRequirement : IAuthorizationRequirement, IAuthorizationHandler
    {
        // This class is intentionally left empty. It serves as a marker for the authorization handler.
        // The actual logic is implemented in the MultiAuthHandler class.
        // You can add properties or methods here if needed in the future.

        // This class implements IAuthorizationRequirement and IAuthorizationHandler interfaces.
        // It serves as a marker for the authorization handler and can be used to define custom authorization requirements.
        // The constructor is intentionally left empty. You can add properties or methods here if needed in the future.

        private readonly string _apiKey;

        public MultiAuthRequirement(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            // ******* JWT *******
            // Check if the user is an Admin
            if (context.User.HasClaim(AuthConstants.AdminUserClaimName, "true"))
            {
                // If the user is an Admin, mark the requirement as succeeded
                context.Succeed(this);
                return;
            }
            
            var httpContext = context.Resource as HttpContext;
            if (httpContext is null)
            {
                context.Fail();
                return;
            }

            // ******* API KEY *******
            var exists = httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var requestApiKey);

            if (exists == false)
            {
                context.Fail();
                return;
            }

            if (requestApiKey.Contains(_apiKey) == false)
            {
                context.Fail();
                return;
            }

            var identity = httpContext.User.Identity! as ClaimsIdentity;
            var userId = requestApiKey.ToString().GetUserIdFromApiKey();
            if (userId is not null)
            {
                identity!.AddClaim(new Claim(AuthConstants.UserIdClaimName, userId.ToString()!)); 
            }
            context.Succeed(this);
        }
    }
}
