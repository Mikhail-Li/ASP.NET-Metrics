using System;
using System.Collections.Generic;
using MetricsManager.Controllers;
using MetricsManager.DAL.Interfaces;
using MetricsManager.Client;
using MetricsManager.DAL.Models;
using MetricsManager.Responses.DTO;
using MetricsManager.Requests;
using MetricsManager.Responses;
using MetricsCommon;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using AutoMapper;
using AutoFixture;

namespace MetricsManagerTests
{
    public class NetworkApiControllerTests
    {
        private readonly NetworkMetricsControllerApi _controllerApi;
        private readonly Mock<INetworkMetricsRepositoryApi> _repositoryApi;
        private readonly Mock<ILogger<NetworkMetricsControllerApi>> _logger;
        private readonly Mock<IMetricsAgentClient> _metricsAgentClient;
        private readonly Mock<IAgentsRepository> _agentsRepository;

        public NetworkApiControllerTests()
        {
            _repositoryApi = new Mock<INetworkMetricsRepositoryApi>();
            _logger = new Mock<ILogger<NetworkMetricsControllerApi>>();
            _metricsAgentClient = new Mock<IMetricsAgentClient>();
            _agentsRepository = new Mock<IAgentsRepository>();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<NetworkMetricsApi, NetworkMetricApiDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time))));
            var mapper = config.CreateMapper();

            _controllerApi = new NetworkMetricsControllerApi(_logger.Object, _metricsAgentClient.Object, _agentsRepository.Object, _repositoryApi.Object, mapper);
        }

        [Fact]
        public void Create_ShouldCall_Create_FromAgent_ReturnOk()
        {
            var fixture = new Fixture();
            var checkList = fixture.Create<AllNetworkMetricApiResponse>();
            var agenturl = fixture.Create<string>();

            _metricsAgentClient.Setup(metrics => metrics.GetAllNetworkMetrics(It.IsAny<GetAllNetworkMetricsApiRequest>())).Returns(checkList);
            _agentsRepository.Setup(agentaddress => agentaddress.GetAgentAddressFromId(It.IsAny<int>())).Returns(agenturl);

            var agentId = 1;
            var fromTime = new DateTimeOffset();
            var toTime = DateTimeOffset.UtcNow;

            var result = (OkObjectResult)_controllerApi.GetMetricsFromAgent(agentId, fromTime, toTime);

            var action = (AllNetworkMetricApiResponse)result.Value;

            _metricsAgentClient.Verify(metrics => metrics.GetAllNetworkMetrics(It.IsAny<GetAllNetworkMetricsApiRequest>()), Times.AtMostOnce());
            _agentsRepository.Verify(agentaddress => agentaddress.GetAgentAddressFromId(It.IsAny<int>()), Times.AtMostOnce());

            Assert.Equal(checkList.Metrics[1].Value, action.Metrics[1].Value);
        }

        [Fact]
        public void Get_ShouldCall_GetfromBase_byPeriod_ReturnOk()
        {
            var fixture = new Fixture();
            var checkList = fixture.Create<List<NetworkMetricsApi>>();

            _repositoryApi.Setup(repository => repository.GetMetricfromBase(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Returns(checkList);

            var agentId = 1;
            var fromTime = new DateTimeOffset();
            var toTime = DateTimeOffset.UtcNow;

            var result = (OkObjectResult)_controllerApi.GetMetricsFromBase(agentId, fromTime, toTime);

            var action = (List<NetworkMetricApiDto>)result.Value;

            _repositoryApi.Verify(repository => repository.GetMetricfromBase(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());

            Assert.Equal(checkList[0].Value, action[0].Value);
        }

        [Fact]
        public void Get_ShouldCall_GetfromBase_byPeriodbyPercentile_ReturnOk()
        {
            var fixture = new Fixture();
            var check = fixture.Create<NetworkMetricsApi>();

            _repositoryApi.Setup(repository => repository.GetMetricbyPercentilefromBase(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<Percentile>())).Returns(check);

            var agentId = 1;
            var fromTime = new DateTimeOffset();
            var toTime = DateTimeOffset.UtcNow;
            var percentile = Percentile.P95;

            var result = (OkObjectResult)_controllerApi.GetMetricsByPercentileFromBase(agentId, fromTime, toTime, percentile);

            var action = (NetworkMetricApiDto)result.Value;

            _repositoryApi.Verify(repository => repository.GetMetricbyPercentilefromBase(It.IsAny<int>(), It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<Percentile>()), Times.AtMostOnce());

            Assert.Equal(check.Value, action.Value);
        }

        [Fact]
        public void Get_ShouldCall_GetfromCluster_byPeriod_ReturnOk()
        {
            var fixture = new Fixture { RepeatCount = 10 };
            var checkList = fixture.Create<List<NetworkMetricsApi>>();

            _repositoryApi.Setup(repository => repository.GetMetricfromClusterfromBase(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Returns(checkList);

            var fromTime = new DateTimeOffset();
            var toTime = DateTimeOffset.UtcNow;

            var result = (OkObjectResult)_controllerApi.GetFromClusterfromBase(fromTime, toTime);

            var action = (List<NetworkMetricApiDto>)result.Value;

            _repositoryApi.Verify(repository => repository.GetMetricfromClusterfromBase(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());

            Assert.Equal(checkList[9].Time, action[9].Time.ToUnixTimeSeconds());
        }

        [Fact]
        public void Get_ShouldCall_GetfromCluster_byPeriodbyPercentile_ReturnOk()
        {
            var fixture = new Fixture();
            var check = fixture.Create<NetworkMetricsApi>();

            _repositoryApi.Setup(repository => repository.GetMetricbyPercentilefromClusterfromBase(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<Percentile>())).Returns(check);

            var fromTime = new DateTimeOffset();
            var toTime = DateTimeOffset.UtcNow;
            var percentile = Percentile.P75;

            var result = (OkObjectResult)_controllerApi.GetByPercentileFromClusterfromBase(fromTime, toTime, percentile);

            var action = (NetworkMetricApiDto)result.Value;

            _repositoryApi.Verify(repository => repository.GetMetricbyPercentilefromClusterfromBase(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<Percentile>()), Times.AtMostOnce());

            Assert.Equal(check.Value, action.Value);
        }
    }
}
