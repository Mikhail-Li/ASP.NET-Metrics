using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using MetricsAgent.DAL.Models;
using MetricsAgent.DAL.Interfaces;
using Dapper;

namespace MetricsAgent.DAL.Repositories
{
    public class RamMetricsRepository : IRamMetricsRepository
    {
        public void Create(RamMetric item)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                connection.Execute("INSERT INTO rammetrics(value, time) VALUES(@value, @time)", new
                {
                    value = item.Value,
                    time = item.Time
                });
            }
        }

        public IList<RamMetric> GetByPeriod(DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                return connection.Query<RamMetric>("SELECT id, value, time FROM rammetrics WHERE time>@fromTime AND time<@toTime", new
                {
                    fromTime = fromTime.ToUnixTimeSeconds(),
                    toTime = toTime.ToUnixTimeSeconds(),
                }).ToList();
            }
        }
    }
}
