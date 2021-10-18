using System;

namespace MetricsAgent.Requests
{
    public class HddMetricRequest
    {
        public DateTimeOffset Time { get; set; }

        public int Value { get; set; }

        public DateTimeOffset FromTime { get; set; }

        public DateTimeOffset ToTime { get; set; }
    }
}
