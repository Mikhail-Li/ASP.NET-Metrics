using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using MetricsAgent.DAL.Models;
using MetricsAgent.DAL.Interfaces;
using Dapper;

namespace MetricsAgent.DAL.Repositories
{
    public class GcHeapSizeMetricsRepository : IGcHeapSizeMetricsRepository
    {
        public void Create(GcHeapSizeMetric item)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                connection.Execute("INSERT INTO gcheapsizemetrics(value, time) VALUES(@value, @time)", new
                {
                    value = item.Value,
                    time = item.Time
                });
            }
        }

        public IList<GcHeapSizeMetric> GetByPeriod(DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                return connection.Query<GcHeapSizeMetric>("SELECT id, value, time FROM gcheapsizemetrics WHERE time>@fromTime AND time<@toTime", new
                {
                    fromTime = fromTime.ToUnixTimeSeconds(),
                    toTime = toTime.ToUnixTimeSeconds(),
                }).ToList();
            }
        }
    }
}
