﻿using System;

namespace MetricsAgent.Responses.DTO
{
    public class RamMetricDto
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}
