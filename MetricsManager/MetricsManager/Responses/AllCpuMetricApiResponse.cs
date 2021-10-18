using System.Collections.Generic;
using MetricsManager.Responses.DTO;


namespace MetricsManager.Responses
{
    public class AllCpuMetricApiResponse
    {
        public List<CpuMetricApiDto> Metrics { get; set; }
    }
}
