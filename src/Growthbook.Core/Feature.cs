using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growthbook.Core
{
    public class Feature
    {
        public JToken DefaultValue { get; set; }

        public IEnumerable<FeatureRule> Rules { get; set; } = Enumerable.Empty<FeatureRule>();
    }
}
