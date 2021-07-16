﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using UIMetricsManager.Request;
using UIMetricsManager.Response;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace UIMetricsManager.Client
{
    public class MetricsManagerClient : IMetricsManagerClient
    {
        public MetricsManagerClient()
        {
        }

        public MetricsResponse GetAllCpuMetrics (MetricRequest request)
        {
            HttpClient client = new HttpClient();
            var toParameter = DateTimeOffset.UtcNow;
            var fromParameter = DateTimeOffset.FromUnixTimeSeconds(toParameter.ToUnixTimeSeconds() - request.PeriodForMetricsGether);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{request.AddressManagerMetrics}/api/metrics/cpu/cluster/from/{fromParameter:O}/to/{toParameter:O}");
            try
            {
                HttpResponseMessage response = client.SendAsync(httpRequest).Result;
                
                var responseStream = response.Content.ReadAsStreamAsync().Result;
                var serialaizer = new JsonSerializer();
                var streamReader = new StreamReader(responseStream, new UTF8Encoding());
                var reader = new JsonTextReader(streamReader);
                
                var resultDTO = new List<MetricApiDTO>();
                resultDTO = serialaizer.Deserialize<List<MetricApiDTO>>(reader);
                
                var result = new MetricsResponse { Metrics = resultDTO };
                
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
