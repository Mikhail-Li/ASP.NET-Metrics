using System.Collections.Generic;
using MetricsManager.Responses.DTO;

namespace MetricsManager.Responses
{
    public class AgentListApiResponse
    {
        public List<AgentDto> Agents { get; set; }
    }
}
