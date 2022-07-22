using Newtonsoft.Json.Linq;

namespace Growthbook.Core
{
    public class GrowthbookFeatureProvider : IFeatureProvider, IAsyncDisposable
    {
        private static readonly object _lock_ = new();

        private readonly HttpClient _httpClient;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly PeriodicTimer _dataRefeshTimer;
        private readonly Task _dataRefreshTask;

        private Dictionary<string, Feature> _data = new();

        private bool _disposed;

        public GrowthbookFeatureProvider(
            HttpClient httpClient,
            string apiKey,
            string baseAddress = "https://cdn.growthbook.io/api/features/",
            TimeSpan? refreshInterval = null)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseAddress + apiKey);
            _dataRefeshTimer = new PeriodicTimer(refreshInterval ?? TimeSpan.FromMinutes(30));
            _dataRefreshTask = HandleTimerAsync(_dataRefeshTimer, _cancellationTokenSource.Token);
        }

        public virtual IReadOnlyDictionary<string, Feature> Data
        {
            get
            {
                lock (_lock_)
                {
                    return _data;
                }
            }
        }

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            string? requestPath = null;
            string responseBody = await _httpClient.GetStringAsync(requestPath, cancellationToken);
            JObject jObject = JObject.Parse(responseBody);

            Dictionary<string, Feature> data = new();
            if (jObject.TryGetValue("features", StringComparison.OrdinalIgnoreCase, out JToken? featuresValue) && featuresValue is JObject features)
            {
                var props = features.Properties();
                foreach (var prop in props)
                {
                    Feature? feature = prop.Value.ToObject<Feature>();
                    if (feature is not null)
                    {
                        data.Add(prop.Name, feature);
                    }
                }
            }

            lock (_lock_)
            {
                _data = data;
            }
        }

        private async Task HandleTimerAsync(PeriodicTimer timer, CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Run(async () => await LoadAsync(cancellationToken), cancellationToken); // Initial load

                while (await timer.WaitForNextTickAsync(cancellationToken))
                {
                    await Task.Run(async () => await LoadAsync(cancellationToken), cancellationToken); // Timer-based loads
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public async ValueTask DisposeAsync()
        {
            Dispose(disposing: true);
            await _dataRefreshTask;
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _cancellationTokenSource.Cancel();
                    _dataRefeshTimer.Dispose();
                    _httpClient.CancelPendingRequests();
                    _httpClient.Dispose();
                    _cancellationTokenSource.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposed = true;
            }
        }
    }
}
