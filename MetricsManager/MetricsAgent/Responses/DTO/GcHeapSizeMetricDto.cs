using System;

namespace MetricsAgent.Responses.DTO
{
    public class GcHeapSizeMetricDto
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}
