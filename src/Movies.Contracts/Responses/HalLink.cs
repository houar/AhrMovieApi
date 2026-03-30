namespace Movies.Contracts.Responses
{
    public class HalLink
    {
        public required string Href { get; init; }
        public required string Rel { get; init; }
        public required string Type { get; init; }
    }
}
