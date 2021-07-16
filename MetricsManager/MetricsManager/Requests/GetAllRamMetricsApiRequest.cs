using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetricsCommon;

namespace MetricsManager.Requests
{
    public class GetAllRamMetricsApiRequest
    {
        public string ClientBaseAddress { get; set; }

        public DateTimeOffset FromTime { get; set; }

        public DateTimeOffset ToTime { get; set; }
    }
}
