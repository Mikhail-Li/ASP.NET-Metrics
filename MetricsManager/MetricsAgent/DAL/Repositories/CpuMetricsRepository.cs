using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SQLite;
using MetricsAgent.DAL.Models;
using MetricsAgent.DAL.Interfaces;
using Dapper;

namespace MetricsAgent.DAL.Repositories
{
    public class CpuMetricsRepository : ICpuMetricsRepository
    {
        public void Create(CpuMetric item)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                connection.Execute("INSERT INTO cpumetrics(value, time) VALUES(@value, @time)", new
                {
                    value = item.Value,
                    time = item.Time
                });
            }
        }

        public IList<CpuMetric> GetByPeriod(DateTimeOffset fromTime, DateTimeOffset toTime)
        {
            using (var connection = new SQLiteConnection(SqlConntectionParameters.сonnection))
            {
                var list =  connection.Query<CpuMetric>("SELECT id, value, time FROM cpumetrics WHERE time>@fromTime AND time<@toTime", new
                {
                    fromTime = fromTime.ToUnixTimeSeconds(),
                    toTime = toTime.ToUnixTimeSeconds(),
                }).ToList();

                return list;
            }
        }
    }
}
