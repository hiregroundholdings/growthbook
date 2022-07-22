using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace Growthbook.Core.Tests
{
    internal class MockFeatureProvider : IFeatureProvider
    {
        private static ConcurrentDictionary<string, Feature>? _data = null;

        public IReadOnlyDictionary<string, Feature> Data
        {
            get
            {
                if (_data is null)
                {
                    const string json = @"{""status"":200,""features"":{""supplier-profile-riskscore-card"":{""defaultValue"":true},""supplier-profile-economicimpact-card"":{""defaultValue"":true},""supplier-search-locationsuggestions"":{""defaultValue"":true},""combined-supplier-create-invite"":{""defaultValue"":true},""combined-supplier-create-invite-checkbox-default-on"":{""defaultValue"":true},""supplier-list-lists"":{""defaultValue"":true},""subscription-popovers"":{""defaultValue"":true},""supplier-export-wait-threshold-seconds"":{""defaultValue"":15},""supplier-list-group"":{""defaultValue"":true},""supplier-list-export-suppliers"":{""defaultValue"":true},""app-theme"":{""defaultValue"":""hireground-classic"",""rules"":[{""force"":""hireground-classic""}]},""customer-branding-signin"":{""defaultValue"":true,""rules"":[{""condition"":{""tenant"":""disabled-tenant-example"",""country"":""US""},""force"":false},{""condition"":{""userId"":""disabled-user-example""},""force"":false},{""condition"":{""tenant"":""a97a0e22-cfc4-43cc-90e1-1a69780f6d2b""},""force"":false}]},""user-onboarding-v2"":{""defaultValue"":true},""user-onboarding-v2-plan-selection"":{""defaultValue"":false,""rules"":[{""variations"":[false,true],""coverage"":1,""weights"":[0.5,0.5],""key"":""user-onboarding-v2-plan-selection"",""hashAttribute"":""tenant""}]}}}";
                    JObject jObject = JObject.Parse(json);

                    _data = new();
                    if (jObject.TryGetValue("features", StringComparison.OrdinalIgnoreCase, out JToken? featuresValue) && featuresValue is JObject features)
                    {
                        var props = features.Properties();
                        foreach (var prop in props)
                        {
                            Feature? feature = prop.Value.ToObject<Feature>();
                            if (feature is not null)
                            {
                                _data.TryAdd(prop.Name, feature);
                            }
                        }
                    }
                }

                return _data;
            }
        }
    }
}
