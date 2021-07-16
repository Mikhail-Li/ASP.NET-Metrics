using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UIMetricsManager.Request;
using UIMetricsManager.Response;
using UIMetricsManager.Client;
using System.Net.Http;

namespace UIMetricsManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IMetricsManagerClient metricClient;
        
        private readonly MetricRequest _request = new MetricRequest { AddressManagerMetrics = "http://localhost:55555", PeriodForMetricsGether = 3 };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object senderr, RoutedEventArgs e)
        {
            metricClient = new MetricsManagerClient();

            MetricsResponse metrics;

            metrics = metricClient.GetAllCpuMetrics(_request);

                foreach (var metric in metrics.Metrics)
                {
                    double valueCpu = metric.Value;

                    if (CpuChart.ColumnServiesValues[0].Values.Count > 9)
                    {
                        CpuChart.ColumnServiesValues[0].Values.RemoveAt(0);
                    }

                    CpuChart.ColumnServiesValues[0].Values.Add(valueCpu);
                }
        }
    }
}
