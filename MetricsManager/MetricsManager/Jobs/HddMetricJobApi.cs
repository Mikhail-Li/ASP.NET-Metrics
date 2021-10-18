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
    public class HddMetricJobApi : IJob
    {
        private readonly IHddMetricsRepositoryApi _repository;
        private readonly IAgentsRepository _agentsRepository;
        private readonly IMetricsAgentClient _agentClient;


        public HddMetricJobApi(IMetricsAgentClient agentClient, IHddMetricsRepositoryApi repository, IAgentsRepository agentsRepository)
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

                var request = new GetAllHddMetricsApiRequest
                {
                    ClientBaseAddress = agent.AgentAddress,
                    FromTime = fromtime,
                    ToTime = totime
                };

                var hddMetrics = _agentClient.GetAllHddMetrics(request);

                if (hddMetrics == null) return Task.CompletedTask;

                var metrics = hddMetrics.Metrics;

                foreach (HddMetricApiDto metric in metrics)
                {
                    var hddMetricToDatabase = new HddMetricsApi { Time = metric.Time.ToUnixTimeSeconds(), Value = metric.Value, AgentId = agent.AgentId };
                    _repository.Create(hddMetricToDatabase);
                }
            }
            return Task.CompletedTask;
        }
    }
}
