using System;
using System.Collections.Generic;
using System.Text;
using MetricsCommon;

namespace MetricsManager.DAL.CommonInterfaces
{
    public interface IRepositoryApi<T> where T : class
    {
        void Create(T item);

        IList<T> GetMetricfromBase(int adentId, DateTimeOffset fromTime, DateTimeOffset toTime);

        T GetMetricbyPercentilefromBase(int agentId, DateTimeOffset fromTime, DateTimeOffset toTime, Percentile percentile);

        IList<T> GetMetricfromClusterfromBase(DateTimeOffset fromTime, DateTimeOffset toTime);

        T GetMetricbyPercentilefromClusterfromBase(DateTimeOffset fromTime, DateTimeOffset toTime, Percentile percentile);

        DateTimeOffset GetLastTime(int agentId);
    }
}
