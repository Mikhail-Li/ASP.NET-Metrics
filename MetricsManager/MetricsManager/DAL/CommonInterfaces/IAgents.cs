using System;
using System.Collections.Generic;
using System.Text;
using MetricsCommon;
using MetricsManager.DAL.Models;

namespace MetricsManager.DAL.CommonInterfaces
{
    public interface IAgents<T> where T : class
    {
        IList<T> GetAgentList();

        void RegisterAgent(T agent);

        string GetAgentAddressFromId(int id);

    }
}
