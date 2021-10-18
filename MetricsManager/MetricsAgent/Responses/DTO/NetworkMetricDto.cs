using System;

namespace MetricsAgent.Responses.DTO
{
    public class NetworkMetricDto
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}
