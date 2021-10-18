using System;
using System.Linq;
using System.Windows;
using System.Threading;
using UIMetricsManager.Request;
using UIMetricsManager.Response;
using UIMetricsManager.Client;
using System.Windows.Threading;

namespace UIMetricsManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IMetricsManagerClient metricClient;

        private double _valueRam;
        private double _valueCpu;
        private double _valueNetwork;

        private readonly int _ramMaxAmountData = 300;
        private readonly int _cpuMaxAmountData = 40;
        private readonly int _networkMaxAmountData = 300;

        private readonly MetricRequest _requestRam = new MetricRequest { AddressManagerMetrics = "http://localhost:55558/api/metrics/ram", PeriodForMetricsGether = 72000 };
        private readonly MetricRequest _requestCpu = new MetricRequest { AddressManagerMetrics = "http://localhost:55558/api/metrics/cpu", PeriodForMetricsGether = 3600 };
        private readonly MetricRequest _requestNetwork = new MetricRequest { AddressManagerMetrics = "http://localhost:55558/api/metrics/network", PeriodForMetricsGether = 3600 };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            metricClient = new MetricsManagerClient();

            MetricsResponse metricsRam = metricClient.GetMetrics(_requestRam);
            MetricsResponse metricsCpu = metricClient.GetMetrics(_requestCpu);
            MetricsResponse metricsNetwork = metricClient.GetMetrics(_requestNetwork);

            Thread ram = new Thread(() => GetRamMetric());
            ram.Start();

            Thread cpu = new Thread(() => GetCpuMetric());
            cpu.Start();

            Thread network = new Thread(() => GetNetworkMetric());
            network.Start();
        }


        public void GetRamMetric()
        {
            Action action = delegate
            {
                if (Chart.RamSeries[0].Values.Count > _ramMaxAmountData)
                {
                    Chart.RamSeries[0].Values.RemoveAt(0);
                };

                Chart.RamSeries[0].Values.Add(_valueRam);
                
            };

            while (true)
            {
                Thread.Sleep(500);

                MetricsResponse metricsRam = metricClient.GetMetrics(_requestRam);
                
                _valueRam = metricsRam.Metrics.Last().Value;

                double sumRamMetrics = 0;

                foreach (var metric in metricsRam.Metrics)
                {
                    sumRamMetrics += metric.Value;
                }

                Chart.RamAverage = Math.Round((sumRamMetrics / metricsRam.Metrics.Count), 1);

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
            }

        }


        public void GetCpuMetric()
        {
            Action action = delegate
            {
                if (Chart.CpuSeries[0].Values.Count > _cpuMaxAmountData)
                {
                    Chart.CpuSeries[0].Values.RemoveAt(0);
                };

                Chart.CpuSeries[0].Values.Add(_valueCpu);
            };

            while (true)
            {
                Thread.Sleep(3000);

                MetricsResponse metricsCpu = metricClient.GetMetrics(_requestCpu);

                _valueCpu = metricsCpu.Metrics.Last().Value;

                double sumCpuMetrics = 0;

                foreach (var metric in metricsCpu.Metrics)
                {
                    sumCpuMetrics += metric.Value;
                }

                Chart.CpuAverage = Math.Round((sumCpuMetrics/ metricsCpu.Metrics.Count), 1);

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
            }
        }

        public void GetNetworkMetric()
        {
            Action action = delegate
            {
                if (Chart.NetworkSeries[0].Values.Count > _networkMaxAmountData)
                {
                    Chart.NetworkSeries[0].Values.RemoveAt(0);
                };

                Chart.NetworkSeries[0].Values.Add(_valueNetwork);
            };

            while (true)
            {
                Thread.Sleep(500);
                
                MetricsResponse metricsNetwork = metricClient.GetMetrics(_requestNetwork);
                
                _valueNetwork = metricsNetwork.Metrics.Last().Value;

                double maxNetworkMetrics = 0;

                foreach (var metric in metricsNetwork.Metrics)
                {
                    if (metric.Value > maxNetworkMetrics) maxNetworkMetrics = metric.Value;
                }

                Chart.NetworkMax = Math.Round((maxNetworkMetrics/1024),1);

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
            }
        }
    }
}
