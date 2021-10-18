using System;
using System.Collections.Generic;
using MetricsAgent.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using MetricsAgent.DAL.Interfaces;
using MetricsAgent.DAL.Models;
using MetricsAgent.Requests;
using MetricsAgent.Responses.DTO;
using MetricsAgent.Responses;
using AutoMapper;
using AutoFixture;

namespace MetricsAgentTests
{
    public class HddControllerTests
    {
        private readonly HddMetricsController _controller;
        private readonly Mock<IHddMetricsRepository> _mock;
        private readonly Mock<ILogger<HddMetricsController>> _loggerMock;
        public HddControllerTests()
        {
            _mock = new Mock<IHddMetricsRepository>();
            _loggerMock = new Mock<ILogger<HddMetricsController>>();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<HddMetric, HddMetricDto>()
               .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time))));
            var mapper = config.CreateMapper();

            _controller = new HddMetricsController(_mock.Object, mapper, _loggerMock.Object);
        }

        [Fact]
        public void Create_ShouldCall_Create_From_Repository()
        {
            _mock.Setup(repository => repository.Create(It.IsAny<HddMetric>())).Verifiable();

            var result = _controller.Create(new HddMetricRequest { Time = DateTimeOffset.UtcNow, Value = 500 });

            _mock.Verify(repository => repository.Create(It.IsAny<HddMetric>()), Times.AtMostOnce());
        }

        [Fact]
        public void GetByPeriod_ShouldCall_GetByPeriod_From_Repository()
        {
            var fixture = new Fixture();
            var checkList = fixture.Create<List<HddMetric>>();

            _mock.Setup(repository => repository.GetByPeriod(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Returns(checkList);

            var fromTime = new DateTimeOffset();
            var toTime = DateTimeOffset.UtcNow;

            var result = (OkObjectResult)_controller.GetByPeriod(fromTime, toTime);

            var action = (AllHddMetricResponse)result.Value;
            var actionResult = action.Metrics;

            _mock.Verify(repository => repository.GetByPeriod(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());

            Assert.Equal(checkList[0].Value, actionResult[0].Value);
        }
    }
}
