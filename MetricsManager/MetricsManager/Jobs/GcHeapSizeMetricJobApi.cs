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
    public class GcHeapSizeMetricJobApi : IJob
    {
        private readonly IGcHeapSizeMetricsRepositoryApi _repository;
        private readonly IAgentsRepository _agentsRepository;
        private readonly IMetricsAgentClient _agentClient;


        public GcHeapSizeMetricJobApi(IMetricsAgentClient agentClient, IGcHeapSizeMetricsRepositoryApi repository, IAgentsRepository agentsRepository)
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

                var request = new GetAllGcHeapSizeMetricsApiRequest
                {
                    ClientBaseAddress = agent.AgentAddress,
                    FromTime = fromtime,
                    ToTime = totime
                };

                var gcHeapSizemetrics = _agentClient.GetAllGcHeapSizeMetrics(request);

                if (gcHeapSizemetrics == null) return Task.CompletedTask;

                var metrics = gcHeapSizemetrics.Metrics;

                foreach (GcHeapSizeMetricApiDto metric in metrics)
                {
                    var gcHeapSizemetricToDatabase = new GcHeapSizeMetricsApi { Time = metric.Time.ToUnixTimeSeconds(), Value = metric.Value, AgentId = agent.AgentId };
                    _repository.Create(gcHeapSizemetricToDatabase);
                }
            }
            return Task.CompletedTask;
        }
    }
}
