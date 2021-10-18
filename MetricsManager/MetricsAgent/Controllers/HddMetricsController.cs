using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using MetricsAgent.DAL.Models;
using MetricsAgent.DAL.Interfaces;
using MetricsAgent.Requests;
using MetricsAgent.Responses;
using MetricsAgent.Responses.DTO;
using AutoMapper;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/hdd")]
    [ApiController]
    public class HddMetricsController : ControllerBase
    {
        private readonly IHddMetricsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<HddMetricsController> _logger;

        public HddMetricsController(IHddMetricsRepository repository, IMapper mapper, ILogger<HddMetricsController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("create")]
        public IActionResult Create([FromRoute] HddMetricRequest request)
        {
            _repository.Create(new HddMetric
            {
                Time = request.Time.ToUnixTimeSeconds(),
                Value = request.Value
            });

            _logger.LogInformation(91, $"HddMetrics method 'Create' Input data: Value = {request.Value}");

            return Ok();
        }

        [HttpGet("from/{fromTime}/to/{toTime}")]
        public IActionResult GetByPeriod([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            IList<HddMetric> metrics = _repository.GetByPeriod(fromTime, toTime);

            var response = new AllHddMetricResponse()
            {
                Metrics = new List<HddMetricDto>()
            };

            if (metrics != null)
            {
                foreach (var metric in metrics)
                {
                    response.Metrics.Add(_mapper.Map<HddMetricDto>(metric));
                }
            }

            _logger.LogInformation(72, $"HddMetrics method 'GetByPerion' execute.");

            return Ok(response);
        }
    }
}

