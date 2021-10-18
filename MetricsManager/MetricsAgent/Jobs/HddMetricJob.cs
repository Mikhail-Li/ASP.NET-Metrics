using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Quartz;
using MetricsAgent.DAL.Interfaces;
using MetricsAgent.DAL.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MetricsAgent.Jobs
{
    public class HddMetricJob : IJob
    {
        private readonly IServiceProvider _provider;
        private readonly IHddMetricsRepository _repository;

        private readonly PerformanceCounter _hddCounter;

        public HddMetricJob(IServiceProvider provider)
        {
            _provider = provider;
            _repository = _provider.GetService<IHddMetricsRepository>();
            _hddCounter = new PerformanceCounter("LogicalDisk", "Free Megabytes", "_Total"); //по всем дискам, также можно в процентах - ("LogicalDisk", "% Free Space", "_Total")
        }
       

        public Task Execute(IJobExecutionContext context)
        {
            var hddLeft = Convert.ToInt32(_hddCounter.NextValue());

            var time = DateTimeOffset.UtcNow;

            _repository.Create(new HddMetric { Time = time.ToUnixTimeSeconds(), Value = hddLeft});

            return Task.CompletedTask;
        }
    }
}
