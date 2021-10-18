using System.Collections.Generic;
using MetricsManager.Responses.DTO;

namespace MetricsManager.Responses
{
    public class AllGcHeapSizeMetricApiResponse
    {
        public List<GcHeapSizeMetricApiDto> Metrics { get; set; }
    }
}
