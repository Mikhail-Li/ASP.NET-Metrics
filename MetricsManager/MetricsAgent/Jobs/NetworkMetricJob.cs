using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Quartz;
using MetricsAgent.DAL.Interfaces;
using MetricsAgent.DAL.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MetricsAgent.Jobs
{
    public class NetworkMetricJob : IJob
    {
        private readonly IServiceProvider _provider;
        private readonly INetworkMetricsRepository _repository;

        private readonly PerformanceCounter _networkCounterWiFi;
        private readonly PerformanceCounter _networkCounterLan;

        public NetworkMetricJob(IServiceProvider provider)
        {
            _provider = provider;
            _repository = _provider.GetService<INetworkMetricsRepository>();
            _networkCounterWiFi = new PerformanceCounter("Network Adapter", "Bytes Total/sec", "Ralink RT3290 802.11bgn Wi-Fi Adapter");
            _networkCounterLan = new PerformanceCounter("Network Adapter", "Bytes Total/sec", "Realtek PCIe FE Family Controller");
        }

        public Task Execute(IJobExecutionContext context)
        {
            var networkUsage = Convert.ToInt32(_networkCounterWiFi.NextValue()) + Convert.ToInt32(_networkCounterLan.NextValue());

            var time = DateTimeOffset.UtcNow;

            _repository.Create(new NetworkMetric { Time = time.ToUnixTimeSeconds(), Value = networkUsage });

            return Task.CompletedTask;
        }
    }
}
