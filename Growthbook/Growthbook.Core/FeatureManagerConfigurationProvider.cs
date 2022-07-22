using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Growthbook.Core
{
    public class FeatureManagerConfigurationProvider : ConfigurationProvider
    {
        private static readonly HttpClient _httpClient = new();

        public FeatureManagerConfigurationProvider(string apiKey, string baseUrl = "https://cdn.growthbook.io/api/features/")
        {
            _httpClient.BaseAddress = new Uri(baseUrl + apiKey);
        }

        public override void Load()
        {
            string? requestPath = null;
            string responseBody = _httpClient.GetStringAsync(requestPath).Result;
            JObject jObject = JObject.Parse(responseBody);

            if (jObject.TryGetValue("features", StringComparison.OrdinalIgnoreCase, out JToken? featuresValue) && featuresValue is JObject features)
            {
                var props = features.Properties();
                foreach (var prop in props)
                {
                    Data.Add(prop.Name, prop.Value.ToString());
                }
            }
        }
    }
}
