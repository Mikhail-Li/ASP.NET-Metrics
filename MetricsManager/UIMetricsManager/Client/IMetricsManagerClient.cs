using UIMetricsManager.Response;
using UIMetricsManager.Request;

namespace UIMetricsManager.Client
{
    public interface IMetricsManagerClient
    {
       MetricsResponse GetMetrics (MetricRequest request);
    }
}