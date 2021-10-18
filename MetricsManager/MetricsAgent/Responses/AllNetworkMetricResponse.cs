using System.Collections.Generic;
using MetricsAgent.Responses.DTO;

namespace MetricsAgent.Responses
{
    public class AllNetworkMetricResponse
    {
        public List<NetworkMetricDto> Metrics { get; set; }
    }
}