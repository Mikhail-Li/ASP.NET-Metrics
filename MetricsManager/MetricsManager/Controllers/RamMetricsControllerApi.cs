using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using MetricsCommon;
using Microsoft.Extensions.Logging;
using MetricsManager.Responses.DTO;
using MetricsManager.Requests;
using MetricsManager.Client;
using MetricsManager.Responses;
using MetricsManager.DAL.Interfaces;
using AutoMapper;

namespace MetricsManager.Controllers
{
    [Route("api/metrics/ram")]
    [ApiController]
    public class RamMetricsControllerApi : ControllerBase
    {
        private readonly ILogger<RamMetricsControllerApi> _logger;
        private readonly IMetricsAgentClient _metricsAgentClient;
        private readonly IAgentsRepository _agentsRepository;
        private readonly IRamMetricsRepositoryApi _repositoryApi;
        private readonly IMapper _mapper;

        public RamMetricsControllerApi(
            ILogger<RamMetricsControllerApi> logger,
            IMetricsAgentClient metricsAgentClient,
            IAgentsRepository agentsRepository,
            IRamMetricsRepositoryApi repositoryApi,
            IMapper mapper)
        {
            _logger = logger;
            _metricsAgentClient = metricsAgentClient;
            _agentsRepository = agentsRepository;
            _repositoryApi = repositoryApi;
            _mapper = mapper;
        }

        /// <summary>
        /// Получает метрики Ram (доступно Мбайт) от Агента сбора метрик на заданном диапазоне времени
        /// </summary>
        /// <remarks>
        /// Пример запросов:
        ///
        ///     GET api/metrics/ram/fromagent/8/from/2021-04-19T20:45:00/to/2021-04-19T20:50:00
        ///     GET api/metrics/ram/fromagent/8/from/2021-04-19/to/2021-04-20
        ///
        /// </remarks>
        /// <param name="agentId">Id зарегистрированного и активного агента </param>
        /// <param name="fromTime">начальная метка времени в формате YYYY-MM-DDThh:mm:ss</param>
        /// <param name="toTime">конечная метка времени в формате YYYY-MM-DDThh:mm:ss</param>
        /// <returns>Список метрик, которые были сохранены Агентом в заданном диапазоне времени</returns>
        /// <response code="200">Если запрос выполнен (или если "metrics": null - агент не зарегистрирован)</response>
        /// <response code="204">Если агент зарегистрироан, но не активен</response>
        /// <response code="400">Если передали не правильные параметры</response>
        [HttpGet("fromagent/{agentId}/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromAgent(
            [FromRoute] int agentId,
            [FromRoute] DateTimeOffset fromTime,
            [FromRoute] DateTimeOffset toTime)
        {

            _logger.LogInformation($"start new request from manager to agent {agentId} ram metrics from {fromTime} to {toTime}");

            var agentUrl = _agentsRepository.GetAgentAddressFromId(agentId);
            
            var metrics = new AllRamMetricApiResponse();
            
            if (agentUrl != null)
            {
                metrics = _metricsAgentClient.GetAllRamMetrics(new GetAllRamMetricsApiRequest
                {
                    ClientBaseAddress = agentUrl,
                    FromTime = fromTime,
                    ToTime = toTime
                });
            }

            if (metrics != null && metrics.Metrics != null)
            {
                for (int i = 0; i < metrics.Metrics.Count; i++)
                {
                    metrics.Metrics[i].AgentId = agentId;
                }
            }
            else
            {
                _logger.LogError("Error with agent.");
            }

            return Ok(metrics);
        }

        /// <summary>
        /// Получает метрики Ram (доступно Мбайт) из базы Менеджера метрик по выбранному Агенту сбора метрик на заданном диапазоне времени
        /// </summary>
        /// <remarks>
        /// Пример запросов:
        ///
        ///     GET api/metrics/ram/frombase/agent/8/from/2021-04-19T20:45:00/to/2021-04-19T20:50:00
        ///     GET api/metrics/ram/frombase/agent/9/from/2021-11-21/to/2021-12-20
        ///
        /// </remarks>
        /// <param name="agentId">Id агента из базы Менеджера метрик</param>
        /// <param name="fromTime">начальная метка времени в формате YYYY-MM-DDThh:mm:ss</param>
        /// <param name="toTime">конечная метка времени в формате YYYY-MM-DDThh:mm:ss</param>
        /// <returns>Список метрик, которые были сохранены в базе Менеджера в заданном диапазоне времени</returns>
        /// <response code="200">Если все хорошо</response>
        /// <response code="400">Если передали не правильные параметры</response>
        [HttpGet("frombase/agent/{agentId}/from/{fromTime}/to/{toTime}")]
        public IActionResult GetMetricsFromBase(
            [FromRoute] int agentId,
            [FromRoute] DateTimeOffset fromTime,
            [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogInformation($"start new request from manager to Base use agent {agentId} ram metrics from {fromTime} to {toTime}");

            var metrics = _repositoryApi.GetMetricfromBase(agentId, fromTime, toTime);
            
            var response = new List<RamMetricApiDto>();
            foreach (var metric in metrics)
            {
                response.Add(_mapper.Map<RamMetricApiDto>(metric));
            }

            return Ok(response);
        }

        /// <summary>
        /// Получает перцентиль Ram (доступно Мбайт) из базы Менеджера метрик по выбранному Агенту сбора метрик на заданном диапазоне времени
        /// </summary>
        /// <remarks>
        /// Пример запросов:
        ///
        ///     GET api/metrics/ram/frombase/agent/8/from/2021-04-19T20:45:00/to/2021-04-19T20:50:00/percentiles/1
        ///     GET api/metrics/ram/frombase/agent/9/from/2021-11-21/to/2021-12-20/percentiles/0
        ///
        /// </remarks>
        /// <param name="agentId">Id агента из базы Менеджера метрик</param>
        /// <param name="fromTime">начальная метка времени в формате YYYY-MM-DDThh:mm:ss</param>
        /// <param name="toTime">конечная метка времени в формате YYYY-MM-DDThh:mm:ss</param>
        /// <param name="percentile">параметр перцентиля (0=Median, 1=P75, 2=P90, 3=P95, 4=P99)</param>
        /// <returns>Список метрик, которые были сохранены в базе Менеджера в заданном диапазоне времени</returns>
        /// <response code="200">Если все хорошо</response>        
        /// <response code="204">Если агент не зарегистрирован или нет данных за указанный период</response>
        /// <response code="400">Если передали не правильные параметры</response>
        [HttpGet("frombase/agent/{agentId}/from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetMetricsByPercentileFromBase(
            [FromRoute] int agentId,
            [FromRoute] DateTimeOffset fromTime,
            [FromRoute] DateTimeOffset toTime,
            [FromRoute] Percentile percentile)
        {
            _logger.LogInformation($"start new request from manager to agent {agentId} ram metrics by Percentile {percentile} from {fromTime} to {toTime}");

            var percentileElement = _repositoryApi.GetMetricbyPercentilefromBase(agentId, fromTime, toTime, percentile);

            RamMetricApiDto response;
            try
            {
                response = _mapper.Map<RamMetricApiDto>(percentileElement);
            }
            catch (Exception ex)
            {
                response = null;
                _logger.LogError(ex.Message);
            }

            return Ok(response);
        }

        /// <summary>
        /// Получает метрики Ram (доступно Мбайт) из базы Менеджера метрик по всем Агентам на заданном диапазоне времени
        /// </summary>
        /// <remarks>
        /// Пример запросов:
        ///
        ///     GET api/metrics/ram/cluster/from/2021-04-19T20:45:00/to/2021-04-19T20:50:00
        ///     GET api/metrics/ram/cluster/from/2021-11-21/to/2021-12-20
        ///
        /// </remarks>
        /// <param name="fromTime">начальная метка времени в формате YYYY-MM-DDThh:mm:ss</param>
        /// <param name="toTime">конечная метка времени в формате YYYY-MM-DDThh:mm:ss</param>
        /// <returns>Список метрик, которые были сохранены в базе Менеджера в заданном диапазоне времени</returns>
        /// <response code="200">Если все хорошо</response>
        /// <response code="400">Если передали не правильные параметры</response>
        [HttpGet("cluster/from/{fromTime}/to/{toTime}")]
        public IActionResult GetFromClusterfromBase(
            [FromRoute] DateTimeOffset fromTime,
            [FromRoute] DateTimeOffset toTime)
        {
            _logger.LogInformation($"This is GetMetricsFromCluster ramMetrics MetricsManager fromTime = {fromTime} toTime = {toTime}.");

            var metrics = _repositoryApi.GetMetricfromClusterfromBase(fromTime, toTime);

            var response = new List<RamMetricApiDto>();
            foreach (var metric in metrics)
            {
                response.Add(_mapper.Map<RamMetricApiDto>(metric));
            }

            return Ok(response);
        }

        /// <summary>
        /// Получает перцентиль Ram (доступно Мбайт) из базы Менеджера метрик среди всех Агентов на заданном диапазоне времени
        /// </summary>
        /// <remarks>
        /// Пример запросов:
        ///
        ///     GET api/metrics/ram/cluster/from/2021-04-19T20:45:00/to/2021-04-19T20:50:00/percentiles/0
        ///     GET api/metrics/ram/cluster/from/2021-11-21/to/2021-12-20/percentiles/4
        ///
        /// </remarks>
        /// <param name="fromTime">начальная метка времени в формате YYYY-MM-DDThh:mm:ss</param>
        /// <param name="toTime">конечная метка времени в формате YYYY-MM-DDThh:mm:ss</param>
        /// <param name="percentile">параметр перцентиля (0=Median, 1=P75, 2=P90, 3=P95, 4=P99)</param>
        /// <returns>Список метрик, которые были сохранены в базе Менеджера в заданном диапазоне времени</returns>
        /// <response code="200">Если все хорошо</response>
        /// <response code="400">Если передали не правильные параметры</response>
        /// <response code="500">Если рассчитать перцентиль невозможно для указанного диапазона</response>
        [HttpGet("cluster/from/{fromTime}/to/{toTime}/percentiles/{percentile}")]
        public IActionResult GetByPercentileFromClusterfromBase(
            [FromRoute] DateTimeOffset fromTime,
            [FromRoute] DateTimeOffset toTime,
            [FromRoute] Percentile percentile)
        {
            _logger.LogInformation($"This is GetByPercentileFromCluster RamMetrics MetricsManager fromTime = {fromTime} toTime = {toTime} percentile = {percentile}.");

            var percentileElement = _repositoryApi.GetMetricbyPercentilefromClusterfromBase(fromTime, toTime, percentile);

            var response = _mapper.Map<RamMetricApiDto>(percentileElement);

            return Ok(response);
        }
    }
}
