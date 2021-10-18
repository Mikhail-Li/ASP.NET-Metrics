using System;

namespace MetricsAgent.DAL.Models
{
    public class GcHeapSizeMetric
    {
        public int Value { get; set; }

        public int Id { get; set; }

        public long Time { get; set; }
        
        public DateTimeOffset Transfer(long inputTime)
        {
            return DateTimeOffset.FromUnixTimeSeconds(inputTime).ToLocalTime();
        }
    }
}
