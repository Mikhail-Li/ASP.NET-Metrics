﻿using System;

namespace MetricsManager.Requests
{
    public class GetAllNetworkMetricsApiRequest
    {
        public string ClientBaseAddress { get; set; }

        public DateTimeOffset FromTime { get; set; }

        public DateTimeOffset ToTime { get; set; }
    }
}
