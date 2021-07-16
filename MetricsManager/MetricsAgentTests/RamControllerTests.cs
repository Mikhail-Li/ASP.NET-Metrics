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
    public class RamControllerTests
    {
        private readonly RamMetricsController _controller;
        private readonly Mock<IRamMetricsRepository> _mock;
        private readonly Mock<ILogger<RamMetricsController>> _loggerMock;

        public RamControllerTests()
        {
            _mock = new Mock<IRamMetricsRepository>();
            _loggerMock = new Mock<ILogger<RamMetricsController>>();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<RamMetric, RamMetricDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time))));
            var mapper = config.CreateMapper();

            _controller = new RamMetricsController(_mock.Object, mapper, _loggerMock.Object);
        }

        [Fact]
        public void Create_ShouldCall_Create_From_Repository()
        {
            _mock.Setup(repository => repository.Create(It.IsAny<RamMetric>())).Verifiable();

            var result = _controller.Create(new RamMetricRequest { Time = DateTimeOffset.UtcNow, Value = 45 });

            _mock.Verify(repository => repository.Create(It.IsAny<RamMetric>()), Times.AtMostOnce());
        }

        [Fact]
        public void GetByPeriod_ShouldCall_GetByPeriod_From_Repository()
        {
            var fixture = new Fixture();
            var checkList = fixture.Create<List<RamMetric>>();

            _mock.Setup(repository => repository.GetByPeriod(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Returns(checkList);

            var fromTime = new DateTimeOffset();
            var toTime = DateTimeOffset.UtcNow;

            var result = (OkObjectResult)_controller.GetByPeriod(fromTime, toTime);

            var action = (AllRamMetricResponse)result.Value;
            var actionResult = action.Metrics;

            _mock.Verify(repository => repository.GetByPeriod(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());

            Assert.Equal(checkList[0].Value, actionResult[0].Value);
        }

    }
}
