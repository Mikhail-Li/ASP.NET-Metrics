using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using MetricsAgent.DAL.Models;
using MetricsAgent.DAL.Interfaces;
using Dapper;

namespace MetricsAgent.DAL.Repositories
{
    public class NetworkMetricsRepository : INetworkMetricsRepository
    {
        public void Create(NetworkMetric item)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                connection.Execute("INSERT INTO networkmetrics(value, time) VALUES(@value, @time)", new
                {
                    value = item.Value,
                    time = item.Time
                });
            }
        }

        public IList<NetworkMetric> GetByPeriod(DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                return connection.Query<NetworkMetric>("SELECT id, value, time FROM networkmetrics WHERE time>@fromTime AND time<@toTime", new
                {
                    fromTime = fromTime.ToUnixTimeSeconds(),
                    toTime = toTime.ToUnixTimeSeconds(),
                }).ToList();
            }
        }
    }
}
