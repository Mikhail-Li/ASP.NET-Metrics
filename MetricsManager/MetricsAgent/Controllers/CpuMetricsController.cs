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
    [Route("api/metrics/cpu")]
    [ApiController]
    public class CpuMetricsController : ControllerBase
    {
        private readonly ICpuMetricsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CpuMetricsController> _logger;

        public CpuMetricsController(ICpuMetricsRepository repository, IMapper mapper, ILogger<CpuMetricsController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("create")]
        public IActionResult Create([FromRoute] CpuMetricRequest request)
        {
            _repository.Create(new CpuMetric
            {
                Time = request.Time.ToUnixTimeSeconds(),
                Value = request.Value
            });
            
            _logger.LogInformation(71, $"CpuMetrics method 'Create' Input data: Time = {request.Time} Value = {request.Value}");
            
            return Ok();
        }

        [HttpGet("from/{fromTime}/to/{toTime}")]
        public IActionResult GetByPeriod([FromRoute] DateTimeOffset fromTime, [FromRoute] DateTimeOffset toTime)
        {
            
            IList<CpuMetric> metrics = _repository.GetByPeriod(fromTime, toTime);

            var response = new AllCpuMetricResponse()
            {
                Metrics = new List<CpuMetricDto>()
            };

            if (metrics != null)
            {
                foreach (var metric in metrics)
                {
                    response.Metrics.Add(_mapper.Map<CpuMetricDto>(metric));
                }
            }

            _logger.LogInformation(72, $"CpuMetrics method 'GetByPerion' execute.");

            return Ok(response);
        }
    }
}
