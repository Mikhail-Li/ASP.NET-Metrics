using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetricsAgent.Responses.DTO;

namespace MetricsAgent.Responses
{
    public class AllGcHeapSizeMetricResponse
    {
        public List<GcHeapSizeMetricDto> Metrics { get; set; }
    }
}
