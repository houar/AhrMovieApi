using System.Text.Json.Serialization;

namespace Movies.Contracts.Responses
{
    public abstract class HalResponse
    {
        private List<HalLink> _links = default!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<HalLink> Links { get { return _links; } }

        public void AddLink(HalLink link)
        {
            if (_links is null)
            {
                _links = new List<HalLink>(); 
            }
            _links.Add(link);
        }
    }
}
