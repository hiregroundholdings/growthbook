namespace Growthbook.Core
{
    public interface IFeatureProvider
    {
        IReadOnlyDictionary<string, Feature> Data { get; }
    }
}
