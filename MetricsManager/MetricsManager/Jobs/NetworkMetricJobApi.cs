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
    public class NetworkMetricJobApi : IJob
    {
        private readonly INetworkMetricsRepositoryApi _repository;
        private readonly IAgentsRepository _agentsRepository;
        private readonly IMetricsAgentClient _agentClient;


        public NetworkMetricJobApi(IMetricsAgentClient agentClient, INetworkMetricsRepositoryApi repository, IAgentsRepository agentsRepository)
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

                var request = new GetAllNetworkMetricsApiRequest
                {
                    ClientBaseAddress = agent.AgentAddress,
                    FromTime = fromtime,
                    ToTime = totime
                };

                var networkMetrics = _agentClient.GetAllNetworkMetrics(request);

                if (networkMetrics == null) return Task.CompletedTask;

                var metrics = networkMetrics.Metrics;

                foreach (NetworkMetricApiDto metric in metrics)
                {
                    var networkMetricToDatabase = new NetworkMetricsApi { Time = metric.Time.ToUnixTimeSeconds(), Value = metric.Value, AgentId = agent.AgentId };
                    _repository.Create(networkMetricToDatabase);
                }
            }
            return Task.CompletedTask;
        }
    }
}
