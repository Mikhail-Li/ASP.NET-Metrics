using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using MetricsManager.DAL.Models;
using MetricsManager.DAL.Interfaces;
using MetricsCommon;
using Dapper;

namespace MetricsManager.DAL.Repositories
{
    public class HddMetricsRepositoryApi : IHddMetricsRepositoryApi
    {
        public void Create(HddMetricsApi item)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                connection.Execute("INSERT INTO hddmetrics(value, time, AgentId) VALUES(@value, @time, @agentId)", new
                {
                    value = item.Value,
                    time = item.Time,
                    agentId = item.AgentId
                });
            }
        }

        public IList<HddMetricsApi> GetMetricfromBase(int agentId, DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                return connection.Query<HddMetricsApi>($"SELECT id, value, time, agentid FROM hddmetrics " +
                    $"WHERE agentid={agentId} AND time>{fromTime.ToUnixTimeSeconds()} AND time<{toTime.ToUnixTimeSeconds()}").ToList();
            }
        }

        public HddMetricsApi GetMetricbyPercentilefromBase(int agentId, DateTimeOffset fromTime, DateTimeOffset toTime, Percentile percentile)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                var metrics = connection.Query<HddMetricsApi>($"SELECT Id, Value, Time, AgentId FROM hddmetrics " +
                    $"WHERE agentid={agentId} AND time>{fromTime.ToUnixTimeSeconds()} AND time<{toTime.ToUnixTimeSeconds()} ORDER BY Value;").ToList();

                var percentileValue = new HddMetricsApi();
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

        public IList<HddMetricsApi> GetMetricfromClusterfromBase(DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                return connection.Query<HddMetricsApi>($"SELECT id, value, time, agentid FROM hddmetrics " +
                    $"WHERE time>{fromTime.ToUnixTimeSeconds()} AND time<{toTime.ToUnixTimeSeconds()}").ToList();
            }
        }

        public HddMetricsApi GetMetricbyPercentilefromClusterfromBase(DateTimeOffset fromTime, DateTimeOffset toTime, Percentile percentile)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                var metrics = connection.Query<HddMetricsApi>($"SELECT Id, Value, Time, AgentId FROM hddmetrics " +
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
                    var time = connection.QuerySingle<long>($"SELECT MAX(`Time`) FROM hddmetrics WHERE AgentId = {agentId};");
                    return DateTimeOffset.FromUnixTimeSeconds(time);
                }
                catch
                {
                    return DateTimeOffset.UnixEpoch;
                }
            }
        }

        private HddMetricsApi GetPercentileValue(List<HddMetricsApi> metrics, int count, Percentile percentile)
        {
            var result = new HddMetricsApi();

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
