using System;
using System.Collections.Generic;
using System.Text;

namespace MetricsManager.DAL.Models
{
    public class CpuMetricsApi
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public long Time { get; set; }

        public int AgentId { get; set; }

        public DateTimeOffset Transfer(long inputTime)
        {
            return DateTimeOffset.FromUnixTimeSeconds(inputTime).ToLocalTime();
        }
    }
}
