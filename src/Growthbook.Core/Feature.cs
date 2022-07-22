using Newtonsoft.Json.Linq;

namespace Growthbook.Core
{
    public class Feature
    {
        public JToken DefaultValue { get; set; }

        public IEnumerable<FeatureRule> Rules { get; set; } = Enumerable.Empty<FeatureRule>();
    }
}
