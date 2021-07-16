using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetricsManager.Responses.DTO;

namespace MetricsManager.Responses
{
    public class AllRamMetricApiResponse
    {
        public List<RamMetricApiDto> Metrics { get; set; }
    }
}
