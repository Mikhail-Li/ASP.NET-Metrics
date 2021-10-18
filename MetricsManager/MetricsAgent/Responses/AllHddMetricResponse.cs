using System.Collections.Generic;
using MetricsAgent.Responses.DTO;

namespace MetricsAgent.Responses
{
    public class AllHddMetricResponse
    {
        public List<HddMetricDto> Metrics { get; set; }
    }
}
