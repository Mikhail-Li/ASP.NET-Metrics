using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UIMetricsManager.Response
{
    public class MetricsResponse
    {
        public List<MetricApiDTO> Metrics { get; set; }
    }

    public class MetricApiDTO
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }

        [JsonProperty("time")]
        public DateTimeOffset Time { get; set; }

        [JsonProperty("agentid")]
        public int AgentId { get; set; }
    }
}
