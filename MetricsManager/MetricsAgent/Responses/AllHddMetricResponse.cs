﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetricsAgent.Responses.DTO;

namespace MetricsAgent.Responses
{
    public class AllHddMetricResponse
    {
        public List<HddMetricDto> Metrics { get; set; }
    }
}