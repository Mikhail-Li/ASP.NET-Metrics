using System;

namespace MetricsAgent.DAL.Models
{
    public class CpuMetric
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public long Time { get; set; }

        public DateTimeOffset Transfer(long inputTime)      //for mapper
        {
            return DateTimeOffset.FromUnixTimeSeconds(inputTime).ToLocalTime();
        }
    }

    
}
