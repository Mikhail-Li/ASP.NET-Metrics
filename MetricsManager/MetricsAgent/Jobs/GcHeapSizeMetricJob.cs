using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Quartz;
using MetricsAgent.DAL.Interfaces;
using MetricsAgent.DAL.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MetricsAgent.Jobs
{
    public class GcHeapSizeMetricJob : IJob
    {
        private readonly IServiceProvider _provider;
        private readonly IGcHeapSizeMetricsRepository _repository;

        private readonly PerformanceCounter _GcHeapSizeCounter;

        public GcHeapSizeMetricJob(IServiceProvider provider)
        {
            _provider = provider;
            _repository = _provider.GetService<IGcHeapSizeMetricsRepository>();
            _GcHeapSizeCounter = new PerformanceCounter(".NET CLR Memory", "# Bytes in all Heaps", "_Global_"); // можно % времени в Gc в (".NET CLR Memory", "% Time in GC", "_Global_")
        }

        public Task Execute(IJobExecutionContext context)
        {
            var dotNet = Convert.ToInt32(_GcHeapSizeCounter.NextValue());

            var time = DateTimeOffset.UtcNow;

            _repository.Create(new GcHeapSizeMetric { Time = time.ToUnixTimeSeconds(), Value = dotNet });

            return Task.CompletedTask;
        }
    }
}
