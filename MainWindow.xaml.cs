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
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.Generic;
using System.Windows;

namespace VizprogProjekt
{
    public partial class MainWindow : Window
    {
        public SeriesCollection PieSeries { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Dummy adatok
            PieSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Termék A",
                    Values = new ChartValues<double> { 40 },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Termék B",
                    Values = new ChartValues<double> { 30 },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Termék C",
                    Values = new ChartValues<double> { 30 },
                    DataLabels = true
                }
            };

            DataContext = this;
        }

        private void telephelyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Ide jön majd az adatbázisos logika
        }
    }
}
