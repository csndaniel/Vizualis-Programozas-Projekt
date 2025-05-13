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
using System.Data.SqlClient;

namespace VizprogProjekt
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Server=localhost;Database=AdventureWorks2022;Trusted_Connection=True;";

        public SeriesCollection CostRateSeries { get; set; }
        public SeriesCollection AvailabilitySeries { get; set; }
        public SeriesCollection QuantitySeries { get; set; }
        public List<string> ProductLabels { get; set; }
        public SeriesCollection GroupedSeries { get; set; }
        public List<string> GroupLabels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadChartData();
            TesztKapcsolat();
            LoadQuantityChart();
            LoadGroupedChart();
            Formatter = value => value.ToString("N2"); 
            DataContext = this;
        }

        private void LoadChartData()
        {
            CostRateSeries = new SeriesCollection();
            AvailabilitySeries = new SeriesCollection();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Name, CostRate, Availability FROM Production.Location WHERE CostRate > 0 OR Availability > 0";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string name = reader["Name"].ToString();
                    double costRate = Convert.ToDouble(reader["CostRate"]);
                    double availability = Convert.ToDouble(reader["Availability"]);

                    if (costRate > 0)
                    {
                        CostRateSeries.Add(new PieSeries
                        {
                            Title = name,
                            Values = new ChartValues<double> { costRate },
                            DataLabels = true
                        });
                    }

                    if (availability > 0)
                    {
                        AvailabilitySeries.Add(new PieSeries
                        {
                            Title = name,
                            Values = new ChartValues<double> { availability },
                            DataLabels = true
                        });
                    }
                }
                reader.Close();
            }
        }

        private void LoadQuantityChart()
        {
            QuantitySeries = new SeriesCollection();
            ProductLabels = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT TOP 5 p.Name, pi.Quantity
            FROM Production.ProductInventory pi
            JOIN Production.Product p ON p.ProductID = pi.ProductID
            ORDER BY pi.Quantity DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                ChartValues<int> quantities = new ChartValues<int>();

                while (reader.Read())
                {
                    string name = reader["Name"].ToString();
                    int quantity = Convert.ToInt32(reader["Quantity"]);

                    ProductLabels.Add(name);
                    quantities.Add(quantity);
                }
                reader.Close();

                QuantitySeries.Add(new ColumnSeries
                {
                    Title = "Darabszám",
                    Values = quantities,
                    DataLabels = true
                });
            }
        }




        private void TesztKapcsolat()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open(); // Megpróbál kapcsolódni az adatbázishoz
                    MessageBox.Show("Sikeres kapcsolat az adatbázissal!", "Kapcsolat Teszt", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az adatbázis kapcsolat során:\n{ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadGroupedChart()
        {
            GroupedSeries = new SeriesCollection();
            GroupLabels = new List<string>();

            ChartValues<decimal> values = new ChartValues<decimal>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT YEAR(TransactionDate) AS Ev, SUM(ActualCost) AS Osszeg
            FROM Production.TransactionHistory
            WHERE YEAR(TransactionDate) IN (2013, 2014)
            GROUP BY YEAR(TransactionDate)
            ORDER BY Ev";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string ev = reader["Ev"].ToString();
                    decimal osszeg = Convert.ToDecimal(reader["Osszeg"]);

                    GroupLabels.Add(ev);
                    values.Add(osszeg);
                }

                reader.Close();
            }

            GroupedSeries.Add(new ColumnSeries
            {
                Title = "Összes költség",
                Values = values,
                DataLabels = true
            });
        }

    }
}
