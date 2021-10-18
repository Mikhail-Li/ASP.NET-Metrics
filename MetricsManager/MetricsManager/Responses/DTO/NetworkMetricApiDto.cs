using System;

namespace MetricsManager.Responses.DTO
{
    public class NetworkMetricApiDto
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public DateTimeOffset Time { get; set; }

        public int AgentId { get; set; }
    }
}
