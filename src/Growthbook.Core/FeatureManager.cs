using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Growthbook.Core
{
    public class FeatureManager
    {
        private readonly IFeatureProvider _provider;
        private readonly Dictionary<string, object> _attributes = new();

        public FeatureManager(IFeatureProvider provider)
        {
            _provider = provider;
        }

        public IReadOnlyDictionary<string, object> Attributes
        {
            get
            {
                return _attributes;
            }
        }

        public IReadOnlyDictionary<string, Feature> Features
        {
            get
            {
                return _provider.Data;
            }
        }

        public FeatureResult<T> EvalFeature<T>(string key)
        {
            if (Features is null || Features.TryGetValue(key, out Feature? feature) == false)
            {
                return new FeatureResult<T>()
                {
                    Source = FeatureResultSource.UnknownFeature
                };
            }

            foreach (FeatureRule rule in feature.Rules)
            {
                if (rule.Condition is not null && !EvalCondition(JObject.FromObject(Attributes), rule.Condition))
                {
                    continue;
                }

                if (rule.Force is not null)
                {
                    return new FeatureResult<T>
                    {
                        Value = rule.Force.ToObject<T>(),
                        Source = FeatureResultSource.Force,
                    };
                }
            }

            return new FeatureResult<T>
            {
                Value = feature.DefaultValue.ToObject<T>(),
                Source = FeatureResultSource.DefaultValue,
            };
        }

        public bool IsOn(string key)
        {
            return EvalFeature<object>(key).On;
        }

        public bool IsOff(string key)
        {
            return EvalFeature<object>(key).Off;
        }

        public void SetAttribute(string key, object? value)
        {
            if (value is null)
            {
                _attributes.Remove(key);
            }
            else if (_attributes.ContainsKey(key))
            {
                _attributes[key] = value;
            }
            else
            {
                _attributes.Add(key, value);
            }
        }

        public T? GetFeatureValue<T>(string key, T fallback)
        {
            FeatureResult<T> result = EvalFeature<T>(key);
            if (result.On)
            {
                return result.Value;
            }

            return fallback;
        }

        private bool EvalAnd(JObject attributes, JToken conditions)
        {
            if (conditions is JArray conditionsArray)
            {
                foreach (JObject condition in conditionsArray.Cast<JObject>())
                {
                    if (!EvalCondition(attributes, condition))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool EvalOr(JObject attributes, JToken conditions)
        {
            if (conditions is JArray conditionsArray)
            {
                if (conditionsArray.Count == 0)
                {
                    return true;
                }

                foreach (JObject condition in conditions.Cast<JObject>())
                {
                    if (EvalCondition(attributes, condition))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool EvalConditionValue(JToken conditionValue, JToken? attributeValue)
        {
            if (conditionValue is JObject valueObject)
            {
                if (IsOperatorObject(valueObject))
                {
                    var props = valueObject.Properties();
                    foreach (JProperty prop in props)
                    {
                        if (attributeValue is not null && !EvalOperatorCondition(prop.Name, attributeValue, conditionValue))
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return JToken.DeepEquals(conditionValue, attributeValue);
            }
        }

        private bool EvalCondition(JObject attributes, JObject condition)
        {

            if (condition.TryGetValue("$or", StringComparison.OrdinalIgnoreCase, out JToken? orValue))
            {
                return EvalOr(attributes, orValue);
            }
            else if (condition.TryGetValue("$nor", StringComparison.OrdinalIgnoreCase, out JToken? norValue))
            {
                return !EvalOr(attributes, norValue);
            }
            else if (condition.TryGetValue("$and", StringComparison.OrdinalIgnoreCase, out JToken? andValue))
            {
                return EvalAnd(attributes, andValue);
            }
            else if (condition.TryGetValue("$not", StringComparison.OrdinalIgnoreCase, out JToken? notValue) && notValue is JObject not)
            {
                return !EvalCondition(attributes, not);
            }

            IEnumerable<JProperty> conditionProps = condition.Properties();
            foreach (JProperty conditionProp in conditionProps)
            {
                string key = conditionProp.Name;
                JToken value = conditionProp.Value;
                if (!EvalConditionValue(value, GetPath(attributes, key)))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ElemMatch(JObject condition, JToken attributeValue)
        {
            if (attributeValue.Type != JTokenType.Array)
            {
                return false;
            }

            JToken[] elements = attributeValue.ToArray();
            foreach (JToken el in elements)
            {
                if (IsOperatorObject(el) && EvalConditionValue(condition, el))
                {
                    return true;
                }
                /*
                else if (EvalCondition(el, condition))
                {
                    return true;
                }
                */
            }

            return false;
        }

        private bool EvalOperatorCondition(string op, JToken attributeValue, JToken conditionValue)
        {
            switch (op)
            {
                case "$eq":
                    return JToken.EqualityComparer.Equals(attributeValue, conditionValue);
                case "$ne":
                    return !JToken.EqualityComparer.Equals(attributeValue, conditionValue);
                case "$lt":
                    throw new NotImplementedException();
                //return attributeValue < conditionValue;
                case "$lte":
                    throw new NotImplementedException();
                //return attributeValue <= conditionValue;
                case "$gt":
                    throw new NotImplementedException();
                //return attributeValue > conditionValue;
                case "$gte":
                    throw new NotImplementedException();
                //return attributeValue >= conditionValue;
                case "$regex":
                    {
                        string input = attributeValue?.ToObject<string>() ?? string.Empty;
                        string pattern = conditionValue.ToObject<string>() ?? string.Empty;
                        return Regex.IsMatch(input, pattern);
                    }
                case "$in":
                    {
                        if (conditionValue is JArray array)
                        {
                            return array.Contains(attributeValue);
                        }
                        else
                        {
                            return false;
                        }
                    }
                case "$nin":
                    {
                        if (conditionValue is JArray array)
                        {
                            return !array.Contains(attributeValue);
                        }
                        else
                        {
                            return false;
                        }
                    }
                case "$size":
                    return EvalConditionValue(conditionValue, attributeValue);
                case "$all":
                    {
                        if (GetAttributeType(attributeValue) != "array")
                        {
                            return false;
                        }

                        return true;
                    }
                case "$exists":
                    {
                        if (conditionValue is null)
                        {
                            return attributeValue is null;
                        }
                        else
                        {
                            return attributeValue is { };
                        }
                    }
                case "$type":
                    return GetAttributeType(attributeValue) == conditionValue.ToObject<string>();
                default:
                    return false;
            }
        }


        private static float GetHash(string input)
        {
            const int offsetBasis = unchecked((int)2166136261);
            const int prime = 16777619;
            float n = input.Aggregate(offsetBasis, (r, o) => (r ^ o.GetHashCode()) * prime);
            return (n % 1000) / 1000;
        }

        private static bool InNamespace(string userId, Tuple<string, float, float> ns)
        {
            (string namespaceId, float rangeStart, float rangeEnd) = ns;
            float n = GetHash(userId + "__" + namespaceId);
            return n >= rangeStart && n < rangeEnd;
        }

        private static JToken? GetPath(JObject attributes, string path)
        {
            return attributes.GetValue(path);
        }

        private static bool IsOperatorObject(JToken? input)
        {
            if (input is JObject @object)
            {
                IEnumerable<JProperty> objProps = @object.Properties();
                foreach (JProperty objProp in objProps)
                {
                    if (!objProp.Name.StartsWith('$'))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private static string GetAttributeType(JToken? input)
        {
            if (input is null)
            {
                return "null";
            }

            switch (input.Type)
            {
                case JTokenType.String:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.Date:
                    return "string";
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.TimeSpan:
                    return "number";
                case JTokenType.Boolean:
                    return "boolean";
                case JTokenType.Array:
                    return "array";
                case JTokenType.Object:
                    return "object";
                case JTokenType.Null:
                    return "null";
                case JTokenType.Undefined:
                    return "undefined";
                case JTokenType.None:
                case JTokenType.Constructor:
                case JTokenType.Property:
                case JTokenType.Comment:
                case JTokenType.Raw:
                case JTokenType.Bytes:
                    return "unknown";
                default:
                    return "unknown";
            }
        }
    }
}