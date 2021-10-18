using System.Collections.Generic;
using MetricsAgent.Responses.DTO;

namespace MetricsAgent.Responses
{
    public class AllCpuMetricResponse
    {
        public List<CpuMetricDto> Metrics { get; set; }
    }
}
