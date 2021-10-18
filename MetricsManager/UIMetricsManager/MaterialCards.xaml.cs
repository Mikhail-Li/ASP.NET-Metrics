using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;

namespace UIMetricsManager
{
    /// <summary>
    /// Interaction logic for MaterialCards.xaml
    /// </summary>
    public partial class MaterialCards : UserControl, INotifyPropertyChanged
    {

        private double _ramAvg;
        private double _cpuAvg;
        private double _networkMax;

        public double RamAverage
        {
            get { return _ramAvg; }
            set
            {
                _ramAvg = value;
                OnPropertyChanged("RamAverage");
            }
        }
        public double CpuAverage
        {
            get { return _cpuAvg; }
            set
            {
                _cpuAvg = value;
                OnPropertyChanged("CpuAverage");
            }
        }

        public double NetworkMax
        {
            get { return _networkMax; }
            set
            {
                _networkMax = value;
                OnPropertyChanged("NetworkMax");
            }
        }
        public SeriesCollection CpuSeries { get; set; }

        public SeriesCollection RamSeries { get; set; }

        public SeriesCollection NetworkSeries { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MaterialCards()
        {
            InitializeComponent();
                          
            RamSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<double> ()
                }
            };

            CpuSeries = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = new ChartValues<double> ()
                }
            };

            NetworkSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<double> ()
                }
            };

            DataContext = this;
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateOnClick (object sender, RoutedEventArgs e)
        {
            TimePowerChart.Update(true);
        }

    }
}
