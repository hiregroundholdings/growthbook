namespace GrowthBook.Core
{
    public class FeatureResult<T>
    {
        public bool On
        {
            get
            {
                if (Value is null)
                {
                    return false;
                }

                string? strValue = Value.ToString();
                return !string.IsNullOrEmpty(strValue) && strValue != "0" && strValue.ToLower() != "false";
            }
        }

        public bool Off
        {
            get
            {
                return !On;
            }
        }

        public FeatureResultSource Source { get; set; }

        public T? Value { get; set; }
    }

    public enum FeatureResultSource
    {
        UnknownFeature,
        DefaultValue,
        Force,
        Experiment,
    }
}
