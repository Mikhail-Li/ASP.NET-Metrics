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
    public class GcHeapSizeControllerTests
    {
        private readonly GcHeapSizeMetricsController _controller;
        private readonly Mock<IGcHeapSizeMetricsRepository> _mock;
        private readonly Mock<ILogger<GcHeapSizeMetricsController>> _loggerMock;

        public GcHeapSizeControllerTests()
        {
            _mock = new Mock<IGcHeapSizeMetricsRepository>();
            _loggerMock = new Mock<ILogger<GcHeapSizeMetricsController>>();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<GcHeapSizeMetric, GcHeapSizeMetricDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time))));
            var mapper = config.CreateMapper();

            _controller = new GcHeapSizeMetricsController(_mock.Object, mapper, _loggerMock.Object);
        }
        
        [Fact]
        public void Create_ShouldCall_Create_From_Repository()
        {
            _mock.Setup(repository => repository.Create(It.IsAny<GcHeapSizeMetric>())).Verifiable();

            var result = _controller.Create(new GcHeapSizeMetricRequest { Time = DateTimeOffset.UtcNow, Value = 40 });

            _mock.Verify(repository => repository.Create(It.IsAny<GcHeapSizeMetric>()), Times.AtMostOnce());
        }

        [Fact]
        public void GetByPeriod_ShouldCall_GetByPeriod_From_Repository()
        {
            var fixture = new Fixture();
            var checkList = fixture.Create<List<GcHeapSizeMetric>>();

            _mock.Setup(repository => repository.GetByPeriod(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>())).Returns(checkList);

            var fromTime = new DateTimeOffset();
            var toTime = DateTimeOffset.UtcNow;

            var result = (OkObjectResult)_controller.GetByPeriod(fromTime, toTime);

            var action = (AllGcHeapSizeMetricResponse)result.Value;
            var actionResult = action.Metrics;

            _mock.Verify(repository => repository.GetByPeriod(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>()), Times.AtMostOnce());
            
            Assert.Equal(checkList[0].Value, actionResult[0].Value);
        }
    }
}