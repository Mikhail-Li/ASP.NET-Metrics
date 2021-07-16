using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using MetricsManager.DAL.Models;
using MetricsManager.DAL.Interfaces;
using MetricsManager.Requests;
using MetricsCommon;
using Dapper;

namespace MetricsManager.DAL.Repositories
{
    public class NetworkMetricsRepositoryApi : INetworkMetricsRepositoryApi
    {
        public void Create(NetworkMetricsApi item)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                connection.Execute("INSERT INTO networkmetrics(value, time, AgentId) VALUES(@value, @time, @agentId)", new
                {
                    value = item.Value,
                    time = item.Time,
                    agentId = item.AgentId
                });
            }
        }

        public IList<NetworkMetricsApi> GetMetricfromBase(int agentId, DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                return connection.Query<NetworkMetricsApi>($"SELECT id, value, time, agentid FROM networkmetrics " +
                    $"WHERE agentid={agentId} AND time>{fromTime.ToUnixTimeSeconds()} AND time<{toTime.ToUnixTimeSeconds()}").ToList();
            }
        }

        public NetworkMetricsApi GetMetricbyPercentilefromBase(int agentId, DateTimeOffset fromTime, DateTimeOffset toTime, Percentile percentile)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                var metrics = connection.Query<NetworkMetricsApi>($"SELECT Id, Value, Time, AgentId FROM networkmetrics " +
                    $"WHERE agentid={agentId} AND time>{fromTime.ToUnixTimeSeconds()} AND time<{toTime.ToUnixTimeSeconds()} ORDER BY Value;").ToList();

                var percentileValue = new NetworkMetricsApi();
                try
                {
                    percentileValue = GetPercentileValue(metrics, metrics.Count, percentile);
                }
                catch
                {
                    percentileValue = null;
                }

                return percentileValue;
            }
        }

        public IList<NetworkMetricsApi> GetMetricfromClusterfromBase(DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                return connection.Query<NetworkMetricsApi>($"SELECT id, value, time, agentid FROM networkmetrics " +
                    $"WHERE time>{fromTime.ToUnixTimeSeconds()} AND time<{toTime.ToUnixTimeSeconds()}").ToList();
            }
        }

        public NetworkMetricsApi GetMetricbyPercentilefromClusterfromBase(DateTimeOffset fromTime, DateTimeOffset toTime, Percentile percentile)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                var metrics = connection.Query<NetworkMetricsApi>($"SELECT Id, Value, Time, AgentId FROM networkmetrics " +
                    $"WHERE time>{fromTime.ToUnixTimeSeconds()} AND time<{toTime.ToUnixTimeSeconds()} ORDER BY Value;").ToList();

                return GetPercentileValue(metrics, metrics.Count, percentile);
            }
        }
        
        public DateTimeOffset GetLastTime(int agentId)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                try
                {
                    var time = connection.QuerySingle<long>($"SELECT MAX(`Time`) FROM networkmetrics WHERE AgentId = {agentId};");
                    return DateTimeOffset.FromUnixTimeSeconds(time);
                }
                catch
                {
                    return DateTimeOffset.UnixEpoch;
                }
            }
        }

        private NetworkMetricsApi GetPercentileValue(List<NetworkMetricsApi> metrics, int count, Percentile percentile)
        {
            var result = new NetworkMetricsApi();

            switch (percentile)
            {
                case Percentile.Median:
                    result = count % 2 == 0 ? metrics[(count / 2) - 1] : metrics[(int)(count / 2)];
                    break;
                case Percentile.P75:
                    result = metrics[(int)(count * 75 / 100) - 1];
                    break;
                case Percentile.P90:
                    result = metrics[(int)(count * 90 / 100) - 1];
                    break;
                case Percentile.P95:
                    result = metrics[(int)(count * 95 / 100) - 1];
                    break;
                case Percentile.P99:
                    result = metrics[(int)(count * 99 / 100) - 1];
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }
    }
}
