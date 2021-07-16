using System;
using System.Collections.Generic;
using MetricsAgent.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using MetricsCommon;
using MetricsAgent.DAL.Interfaces;
using MetricsAgent.DAL.Models;
using MetricsAgent.Requests;
using MetricsAgent.Responses.DTO;
using MetricsAgent.Responses;
using AutoMapper;
using AutoFixture;

namespace MetricsAgentTests
{
    public class CpuControllerTests
    {
        private readonly CpuMetricsController _controller;
        private readonly Mock<ICpuMetricsRepository> _mock;
        private readonly Mock<ILogger<CpuMetricsController>> _loggerMock;

        public CpuControllerTests()
        {
            _mock = new Mock<ICpuMetricsRepository>();
            _loggerMock = new Mock<ILogger<CpuMetricsController>>();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<CpuMetric, CpuMetricDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time))));
            var mapper = config.CreateMapper();

            _controller = new CpuMetricsController(_mock.Object, mapper, _loggerMock.Object);
        }

        [Fact]
        public void Create_ShouldCall_Create_From_Repository()
        {
            _mock.Setup(repository => repository.Create(It.IsAny<CpuMetric>())).Verifiable();

            var result = _controller.Create(new CpuMetricRequest { Time = DateTimeOffset.UtcNow, Value = 50 });

            _mock.Verify(repository => repository.Create(It.IsAny<CpuMetric>()), Times.AtMostOnce());
        }

        [Fact]
        public void GetByPeriod_ShouldCall_GetByPeriod_From_Repository()
        {
            var fixture = new Fixture();
            var checkList = fixture.Create<List<CpuMetric>>();

            _mock.Setup(repository => repository.GetByPeriod(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Returns(checkList);

            var fromTime = new DateTimeOffset();
            var toTime = DateTimeOffset.UtcNow;

            var result = (OkObjectResult)_controller.GetByPeriod( fromTime, toTime);

            var action = (AllCpuMetricResponse)result.Value;
            var actionResult = action.Metrics;

            _mock.Verify(repository => repository.GetByPeriod(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());
            
            Assert.Equal(checkList[0].Value, actionResult[0].Value);
        }
    }
}


