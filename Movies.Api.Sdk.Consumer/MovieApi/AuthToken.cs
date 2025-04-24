using System.IdentityModel.Tokens.Jwt;

namespace Movies.Api.Sdk.Consumer.MovieApi
{
    internal class AuthToken
    {
        private string _token;
        public string BearerToken { get { return _token; } }

        private DateTime _exp;
        public bool Expired { get { return _exp.AddSeconds(-5) > DateTime.UtcNow; } }

        public AuthToken(string token)
        {
            _token = token;
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var exp = jwt.Claims.Single(c => c.Type == "exp");
            _exp = UnixTimeStampToDateTime(exp.Value);
        }

        private static DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            var seconds = long.Parse(unixTimeStamp);
            var dateTime = DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
            return dateTime;
        }
    }
}
