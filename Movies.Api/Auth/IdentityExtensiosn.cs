namespace Movies.Api.Auth
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

        internal static Guid? GetUserIdFromApiKey(this string apiKey)
        {
            //For the sake of the demo, we are assuming that the API key is a User GUID.
            if (Guid.TryParse(apiKey, out var guid))
            {
                return guid;
            }
            return null;
        }
    }
}
