using System.Collections.Generic;
using MetricsManager.Responses.DTO;

namespace MetricsManager.Responses
{
    public class AllNetworkMetricApiResponse
    {
        public List<NetworkMetricApiDto> Metrics { get; set; }
    }
}
