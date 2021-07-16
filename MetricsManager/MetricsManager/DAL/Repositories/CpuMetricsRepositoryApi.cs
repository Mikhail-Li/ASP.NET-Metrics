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
    public class CpuMetricsRepositoryApi:ICpuMetricsRepositoryApi
    {
        public void Create(CpuMetricsApi item)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                
                connection.Execute("INSERT INTO cpumetrics(value, time, AgentId) VALUES(@value, @time, @agentId)", new
                {
                    value = item.Value,
                    time = item.Time,
                    agentId = item.AgentId
                });
            }
        }

        public IList<CpuMetricsApi> GetMetricfromBase(int agentId, DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                return connection.Query<CpuMetricsApi>($"SELECT id, value, time, agentid FROM cpumetrics WHERE agentid={agentId} AND time>{fromTime.ToUnixTimeSeconds()} AND time<{toTime.ToUnixTimeSeconds()}").ToList();
            }
        }

        public CpuMetricsApi GetMetricbyPercentilefromBase(int agentId, DateTimeOffset fromTime, DateTimeOffset toTime, Percentile percentile)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                var metrics = connection.Query<CpuMetricsApi>($"SELECT Id, Value, Time, AgentId FROM cpumetrics " +
                    $"WHERE agentid={agentId} AND time>{fromTime.ToUnixTimeSeconds()} AND time<{toTime.ToUnixTimeSeconds()} ORDER BY Value;").ToList();

                var percentileValue = new CpuMetricsApi();
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

        public IList<CpuMetricsApi> GetMetricfromClusterfromBase(DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                return connection.Query<CpuMetricsApi>($"SELECT id, value, time, agentid FROM cpumetrics WHERE time>{fromTime.ToUnixTimeSeconds()} AND time<{toTime.ToUnixTimeSeconds()}").ToList();
            }
        }

        public CpuMetricsApi GetMetricbyPercentilefromClusterfromBase(DateTimeOffset fromTime, DateTimeOffset toTime, Percentile percentile)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection)) 
            {
                var metrics = connection.Query<CpuMetricsApi>($"SELECT Id, Value, Time, AgentId FROM cpumetrics " +
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
                    var time = connection.QuerySingle<long>($"SELECT MAX(`Time`) FROM cpumetrics WHERE AgentId = {agentId};");
                    return DateTimeOffset.FromUnixTimeSeconds(time);
                }
                catch
                {
                    return DateTimeOffset.UnixEpoch;
                }
            }
        }

        private CpuMetricsApi GetPercentileValue(List<CpuMetricsApi> metrics, int count, Percentile percentile)
        {
            var result = new CpuMetricsApi();

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
