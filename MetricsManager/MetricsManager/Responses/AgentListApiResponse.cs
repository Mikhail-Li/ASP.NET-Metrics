using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetricsManager.Responses.DTO;

namespace MetricsManager.Responses
{
    public class AgentListApiResponse
    {
        public List<AgentDto> Agents { get; set; }
    }
}
