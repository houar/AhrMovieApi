namespace Movies.Api.Auth
{
    internal class AuthConstants
    {
        internal const string AdminUserPolicyName = "Admin";
        internal const string AdminUserClaimName = "admin";
        internal const string AdminUserClaimValue = "true";

        internal const string AdminOrTrustedPolicyName = "Trusted";
        internal const string TrustedMemberClaimName = "trusted_member";
        internal const string TrustedMemberClaimValue = "true";

        internal const string UserIdClaimName = "userid";

        internal const string ApiKeyHeaderName = "x-api-key";
    }
}
