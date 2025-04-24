using System.Net.Http.Headers;

namespace Movies.Api.Sdk.Consumer.MovieApi
{
    internal class MovieApiPostHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // If there's content (e.g., POST, PUT), add the media type version param
            if (request.Content != null)
            {
                var contentType = new MediaTypeHeaderValue("application/json");
                contentType.Parameters.Add(new NameValueHeaderValue("x-api-version", "2.0"));

                request.Content.Headers.ContentType = contentType;
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
