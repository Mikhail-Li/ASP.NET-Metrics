using System;

namespace MetricsManager.Requests
{
    public class GetAllRamMetricsApiRequest
    {
        public string ClientBaseAddress { get; set; }

        public DateTimeOffset FromTime { get; set; }

        public DateTimeOffset ToTime { get; set; }
    }
}
