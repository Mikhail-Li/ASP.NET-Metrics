﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsManager.DAL.Models
{
    public class GcHeapSizeMetricsApi
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
