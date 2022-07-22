using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growthbook.Core.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddGrowthbookConfiguration(this IConfigurationBuilder configurationBuilder, string apiKey, string baseUrl = "https://cdn.growthbook.io/api/features/")
        {
            return configurationBuilder.Add(new FeatureManagerConfigurationSource(apiKey, baseUrl));
        }
    }
}
