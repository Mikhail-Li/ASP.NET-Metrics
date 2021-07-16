using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
