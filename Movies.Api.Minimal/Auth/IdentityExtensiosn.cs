namespace Movies.Api.Minimal.Auth
{
    public static class IdentityExtensiosn
    {
        public static Guid? GetUserId(this HttpContext context)
        {
            var userId = context.User.Claims.FirstOrDefault(c => c.Type == AuthConstants.UserIdClaimName);

            if (Guid.TryParse(userId?.Value, out var guid))
            {
                return guid;
            }
            return null;
        }
    }
}
