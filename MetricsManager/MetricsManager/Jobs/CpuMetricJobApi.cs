using System;
using System.Threading.Tasks;
using Quartz;
using MetricsManager.DAL.Interfaces;
using MetricsManager.Responses.DTO;
using MetricsManager.Requests;
using MetricsManager.DAL.Models;
using MetricsManager.Client;

namespace MetricsManager.Jobs
{
    [DisallowConcurrentExecution]
    public class CpuMetricJobApi : IJob
    {
        private readonly ICpuMetricsRepositoryApi _repository;
        private readonly IAgentsRepository _agentsRepository;
        private readonly IMetricsAgentClient _agentClient;


        public CpuMetricJobApi(IMetricsAgentClient agentClient, ICpuMetricsRepositoryApi repository, IAgentsRepository agentsRepository)
        {
            _repository = repository;  
            _agentsRepository = agentsRepository;
            _agentClient = agentClient;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var agentList = _agentsRepository.GetAgentList();

            if (agentList == null) return Task.CompletedTask;

            foreach (AgentInfo agent in agentList)
            {
                DateTimeOffset fromtime = _repository.GetLastTime(agent.AgentId); 
                DateTimeOffset totime = DateTimeOffset.UtcNow;

                var request = new GetAllCpuMetricsApiRequest
                {
                    ClientBaseAddress = agent.AgentAddress,
                    FromTime = fromtime,
                    ToTime = totime
                };

                var cpuMetrics = _agentClient.GetAllCpuMetrics(request);

                if (cpuMetrics == null) return Task.CompletedTask;

                var metrics = cpuMetrics.Metrics;

                foreach (CpuMetricApiDto metric in metrics)
                {
                    var cpuMetricToDatabase = new CpuMetricsApi { Time = metric.Time.ToUnixTimeSeconds(), Value = metric.Value, AgentId = agent.AgentId };
                    _repository.Create(cpuMetricToDatabase);
                }
            }
            return Task.CompletedTask;
        }
    }
}
