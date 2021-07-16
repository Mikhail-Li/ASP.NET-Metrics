using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIMetricsManager.Response;
using UIMetricsManager.Request;

namespace UIMetricsManager.Client
{
    public interface IMetricsManagerClient
    {
       MetricsResponse GetAllCpuMetrics (MetricRequest request);
    }
}