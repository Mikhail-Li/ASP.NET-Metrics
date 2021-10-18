using System.Collections.Generic;
using MetricsManager.Responses.DTO;

namespace MetricsManager.Responses
{
    public class AllHddMetricApiResponse
    {
        public List<HddMetricApiDto> Metrics { get; set; }
    }
}
