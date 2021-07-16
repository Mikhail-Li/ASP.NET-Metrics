using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Quartz;
using MetricsManager.DAL.Interfaces;
using MetricsManager.DAL.Repositories;
using MetricsManager.Responses;
using MetricsManager.Responses.DTO;
using MetricsManager.Requests;
using MetricsManager.DAL.Models;
using MetricsManager.Client;
using Microsoft.Extensions.DependencyInjection;
using Dapper;

namespace MetricsManager.Jobs
{
    [DisallowConcurrentExecution]
    public class RamMetricJobApi : IJob
    {
        private readonly IRamMetricsRepositoryApi _repository;
        private readonly IAgentsRepository _agentsRepository;
        private readonly IMetricsAgentClient _agentClient;


        public RamMetricJobApi(IMetricsAgentClient agentClient, IRamMetricsRepositoryApi repository, IAgentsRepository agentsRepository)
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

                var request = new GetAllRamMetricsApiRequest
                {
                    ClientBaseAddress = agent.AgentAddress,
                    FromTime = fromtime,
                    ToTime = totime
                };

                var ramMetrics = _agentClient.GetAllRamMetrics(request);

                if (ramMetrics == null) return Task.CompletedTask;

                var metrics = ramMetrics.Metrics;

                foreach (RamMetricApiDto metric in metrics)
                {
                    var ramMetricToDatabase = new RamMetricsApi { Time = metric.Time.ToUnixTimeSeconds(), Value = metric.Value, AgentId = agent.AgentId };
                    _repository.Create(ramMetricToDatabase);
                }
            }
            return Task.CompletedTask;
        }
    }
}
