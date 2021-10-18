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
    [Route("api/metrics/network")]
    [ApiController]
    public class NetworkMetricsController : ControllerBase
    {
        private readonly INetworkMetricsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<NetworkMetricsController> _logger;

        public NetworkMetricsController(INetworkMetricsRepository repository, IMapper mapper, ILogger<NetworkMetricsController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }
        
        [HttpPost("create")]
        public IActionResult Create([FromRoute] NetworkMetricRequest request)
        {
            _repository.Create(new NetworkMetric
            {
                Time = request.Time.ToUnixTimeSeconds(),
                Value = request.Value
            });

            _logger.LogInformation(101, $"NetworkMetrics method 'Create' Input data: Time = {request.Time} Value = {request.Value}");

            return Ok();
        }

        [HttpGet("from/{fromTime}/to/{toTime}")]
        public IActionResult GetByPeriod([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            IList<NetworkMetric> metrics = _repository.GetByPeriod(fromTime, toTime);

            var response = new AllNetworkMetricResponse()
            {
                Metrics = new List<NetworkMetricDto>()
            };

            if (metrics != null) 
            {
                foreach (var metric in metrics)
                {
                    response.Metrics.Add(_mapper.Map<NetworkMetricDto>(metric));
                }
            }

            _logger.LogInformation(102, $"NetworkMetrics method 'GetByPerion' execute.");

            return Ok(response);
        }
    }
}
