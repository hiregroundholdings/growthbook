using Newtonsoft.Json.Linq;

namespace Growthbook.Core
{
    public class FeatureRule
    {
        /// <summary>
        /// Optional targeting condition
        /// </summary>
        public JObject? Condition { get; set; }

        /// <summary>
        /// What percent of users should be included in the experiment (between 0 and 1, inclusive)
        /// </summary>
        public double? Coverage { get; set; }

        /// <summary>
        /// Immediately force a specific value (ignore every other option besides condition and coverage)
        /// </summary>
        public JToken? Force { get; set; }

        /// <summary>
        /// Run an experiment (A/B test) and randomly choose between these variations
        /// </summary>
        public IReadOnlyList<object>? Variations { get; set; }

        /// <summary>
        /// How to weight traffic between variations. Must add to 1.
        /// </summary>
        public IReadOnlyList<double>? Weights { get; set; }

        /// <summary>
        /// The globally unique tracking key for the experiment (default to the feature key)
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// What user attribute should be used to assign variations (defaults to id)
        /// </summary>
        public string HashAttribute { get; set; } = "id";

        /// <summary>
        /// Adds the experiment to a namespace
        /// </summary>
        public Tuple<string, float, float>? Namespace { get; set; }
    }
}
