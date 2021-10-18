using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using MetricsManager.DAL.Models;
using MetricsManager.DAL.Interfaces;
using Dapper;

namespace MetricsManager.DAL.Repositories
{
    public class AgentsRepository : IAgentsRepository
    {
        public void RegisterAgent(AgentInfo agent)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                connection.Execute("INSERT INTO agents(agentUrl) VALUES(@agentUrl)", new { agentUrl = agent.AgentAddress });
            }
        }

        public IList<AgentInfo> GetAgentList()
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                var agentlistid = connection.Query<int>("SELECT AgentId FROM agents;").ToList();
                var agentlisturl = connection.Query<string>("SELECT AgentUrl FROM agents;").ToList();
                var agentlist = new List<AgentInfo>();
                int i = 0;
                foreach (int agentid in agentlistid)
                {
                    var agent = new AgentInfo { AgentId = agentid, AgentAddress = agentlisturl[i] };
                    agentlist.Add(agent);
                    i++;
                }
                return agentlist;
            }
        }

        public string GetAgentAddressFromId (int id)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                string agentUrl;

                try
                {
                    agentUrl = connection.QuerySingle<string>($"SELECT AgentUrl FROM agents WHERE AgentId={id}");
                }
                catch
                {
                    agentUrl = null;
                }
                return agentUrl;
            }
        }
    }
}
