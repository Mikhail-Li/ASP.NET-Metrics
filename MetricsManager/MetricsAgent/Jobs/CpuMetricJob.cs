using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Quartz;
using MetricsAgent.DAL.Interfaces;
using MetricsAgent.DAL.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MetricsAgent.Jobs
{
    public class CpuMetricJob : IJob
    {
        private readonly IServiceProvider _provider;
        private readonly ICpuMetricsRepository _repository;

        private readonly PerformanceCounter _cpuCounter;

        public CpuMetricJob(IServiceProvider provider)
        {
            _provider = provider;
            _repository = _provider.GetService<ICpuMetricsRepository>();
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        public Task Execute(IJobExecutionContext context)
        {
            var cpuUsageInPercents = Convert.ToInt32(_cpuCounter.NextValue());

            var time = DateTimeOffset.UtcNow;

            _repository.Create(new CpuMetric { Time = time.ToUnixTimeSeconds(), Value = cpuUsageInPercents });

            return Task.CompletedTask;
        }
    }
}
