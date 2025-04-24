namespace Movies.Api.Sdk.Consumer.MovieApi
{
    internal class MovieApiOptions
    {
        public string BaseUrl { get; set; }
        public string BaseUrlBackup { get; set; }
        public string ApiVersion { get; set; }
        public int WaitTimeInMilliseconds { get; set; }
        public int Timeout { get; set; }
        public string AuthEndpoint { get; set; }
    }
}
