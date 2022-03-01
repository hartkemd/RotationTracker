using DataAccessLibrary.Models;
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
using System.Windows.Shapes;

namespace RotationTracker
{
    /// <summary>
    /// Interaction logic for Coverages.xaml
    /// </summary>
    public partial class Coverages : Window
    {
        private MainWindow _parentWindow;

        public Coverages(MainWindow parentWindow)
        {
            InitializeComponent();

            _parentWindow = parentWindow;
            coveragesListBox.ItemsSource = _parentWindow.coverages;
            coveringEmployeeComboBox.ItemsSource = _parentWindow.employees;
        }

        private void FilterCoveragesButton_Click(object sender, RoutedEventArgs e)
        {
            List<CoverageReadModel> coverageReads = new List<CoverageReadModel>();
            string coveringEmployee = coveringEmployeeComboBox.Text;

            coverageReads = _parentWindow.coverages.Where(x => x.EmployeeCovering == coveringEmployee).ToList();
            coveragesListBox.ItemsSource = coverageReads;
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            coveragesListBox.ItemsSource = _parentWindow.coverages;
        }
    }
}
