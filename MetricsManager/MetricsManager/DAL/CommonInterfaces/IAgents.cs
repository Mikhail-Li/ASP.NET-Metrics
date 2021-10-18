using System.Collections.Generic;

namespace MetricsManager.DAL.CommonInterfaces
{
    public interface IAgents<T> where T : class
    {
        IList<T> GetAgentList();

        void RegisterAgent(T agent);

        string GetAgentAddressFromId(int id);

    }
}
