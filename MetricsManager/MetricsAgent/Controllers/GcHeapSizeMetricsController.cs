using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using MetricsCommon;
using MetricsAgent.DAL.Models;
using MetricsAgent.DAL.Interfaces;
using MetricsAgent.Requests;
using MetricsAgent.Responses;
using MetricsAgent.Responses.DTO;
using AutoMapper;

namespace MetricsAgent.Controllers
{
    [Route("api/metrics/gcheapsize")]
    [ApiController]
    
    public class GcHeapSizeMetricsController : ControllerBase
    {
        private readonly IGcHeapSizeMetricsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GcHeapSizeMetricsController> _logger;

        public GcHeapSizeMetricsController(IGcHeapSizeMetricsRepository repository, IMapper mapper, ILogger<GcHeapSizeMetricsController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("create")]
        public IActionResult Create([FromRoute] GcHeapSizeMetricRequest request)
        {
            _repository.Create(new GcHeapSizeMetric
            {
                Time =request.Time.ToUnixTimeSeconds(),
                Value = request.Value
            });

            _logger.LogInformation(81, $"GcHeapSizeMetrics method 'Create' Input data: Time = {request.Time} Value = {request.Value}");

            return Ok();
        }

        [HttpGet("from/{fromTime}/to/{toTime}")]
        public IActionResult GetByPeriod([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            IList<GcHeapSizeMetric> metrics = _repository.GetByPeriod(fromTime, toTime);

            var response = new AllGcHeapSizeMetricResponse()
            {
                Metrics = new List<GcHeapSizeMetricDto>()
            };

            if (metrics != null) 
            {
                foreach (var metric in metrics)
                {
                    response.Metrics.Add(_mapper.Map<GcHeapSizeMetricDto>(metric));
                }
            }

            _logger.LogInformation(82, $"GcHeapSizeMetric method 'GetByPerion' execute.");

            return Ok(response);
        }
    }
}

