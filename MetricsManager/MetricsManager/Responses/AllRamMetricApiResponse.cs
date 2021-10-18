using System.Collections.Generic;
using MetricsManager.Responses.DTO;

namespace MetricsManager.Responses
{
    public class AllRamMetricApiResponse
    {
        public List<RamMetricApiDto> Metrics { get; set; }
    }
}
