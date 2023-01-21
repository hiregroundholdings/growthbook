using Newtonsoft.Json.Linq;

namespace GrowthBook.Core
{
    public class Feature
    {
        public JToken DefaultValue { get; set; }

        public IEnumerable<FeatureRule> Rules { get; set; } = Enumerable.Empty<FeatureRule>();
    }
}
