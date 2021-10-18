using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MetricsManager.DAL.Models;
using MetricsManager.DAL.Interfaces;
using MetricsManager.Responses;
using MetricsManager.Responses.DTO;
using AutoMapper;

namespace MetricsManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<AgentsController> _logger;

        public AgentsController(IAgentsRepository repository, IMapper mapper, ILogger<AgentsController> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Регистрирует Агента сбора метрик с заданным Url
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///     
        ///     POST /api/agents/register
        /// 
        ///     в теле (body) указывают:
        ///     { 
        ///         "аgentId": 0,
        ///         "agentAddress": "http://localhost:6555" 
        ///     }* 
        ///     
        /// * поле "аgentId" приваевается автоматически: очередной, следующий за максимальным существующим. 
        ///   Если указать существующий, оставить незаполненным или убрать из body, то ошибки не будет. 
        /// 
        /// </remarks>
        /// <returns>OK</returns>
        /// <response code="200">Если агент зарегистрирован</response>
        /// <response code="400">Если передали не правильные параметры</response>

        [HttpPost("register")]
        public IActionResult RegisterAgent([FromBody] AgentInfo agent)
        {
            _repository.RegisterAgent(agent);

            _logger.LogInformation(11, "This is RegisterAgent.");
            _logger.LogInformation(11, $"Аgent Id = {agent.AgentId} Address = {agent.AgentAddress} is registered.");

            return Ok();
        }

        /// <summary>
        /// Показывает список зарегистрированных Агентов сбора метрик
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET /api/agents/agentlist
        ///     
        /// </remarks>
        /// <returns>OK</returns>
        /// <response code="200">Если список получен и выведен</response>
        /// <response code="400">Если ошибка</response>
        [HttpGet("agentlist")] 
        public IActionResult GetAgentList()
        {
            IList<AgentInfo> agents = _repository.GetAgentList();

            var response = new AgentListApiResponse()
            {
                Agents = new List<AgentDto>()
            };

            foreach (var agent in agents)
            {
                response.Agents.Add(_mapper.Map<AgentDto>(agent));
            }

            _logger.LogInformation(12, $" GetAgentList execute.");

            return Ok(response);
        }


        /// <summary>
        /// Показывает адрес зарегистрированного Агента по его Id
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET /api/agents/agentaddress/6
        ///     
        /// </remarks>
        /// <returns>OK</returns>
        /// <response code="200">Если агент найден с адресом</response>
        /// <response code="204">Если агент не зарегистрирован</response>
        /// <response code="400">Если передали не правильные параметры</response>
        [HttpGet("agentaddress/{id}")]
        public IActionResult GetAgentAddressFromId ([FromRoute] int id)
        {
            return Ok(_repository.GetAgentAddressFromId(id));
        }
    }
}
