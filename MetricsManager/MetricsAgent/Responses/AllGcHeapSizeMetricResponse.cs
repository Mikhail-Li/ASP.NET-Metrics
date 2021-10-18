using System.Collections.Generic;
using MetricsAgent.Responses.DTO;

namespace MetricsAgent.Responses
{
    public class AllGcHeapSizeMetricResponse
    {
        public List<GcHeapSizeMetricDto> Metrics { get; set; }
    }
}
