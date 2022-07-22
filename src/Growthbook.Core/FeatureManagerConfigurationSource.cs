using Microsoft.Extensions.Configuration;

namespace Growthbook.Core
{
    public class FeatureManagerConfigurationSource : IConfigurationSource
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public FeatureManagerConfigurationSource(
            string apiKey,
            string baseUrl = "https://cdn.growthbook.io/api/features/")
        {
            _apiKey = apiKey;
            _baseUrl = baseUrl;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new FeatureManagerConfigurationProvider(_apiKey, _baseUrl);
        }
    }
}
