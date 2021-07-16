using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        public MaterialCards()
        {
            InitializeComponent();

            ColumnServiesValues = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = new ChartValues<double> ()
                }
            };
            DataContext = this;
        }

        public SeriesCollection ColumnServiesValues { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

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
