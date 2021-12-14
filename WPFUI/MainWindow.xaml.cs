using JSONFileIOLibrary;
using RotationLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public EmployeeListModel employeeList = new();
        public string employeeListFilePath = "EmployeeList.json";
        public string rotation1FilePath = "Rotation1.json";

        public RotationModel rotation1 = new();

        public MainWindow()
        {
            InitializeComponent();

            employeeList = JSONFileIO.Load<EmployeeListModel>(employeeListFilePath);
            rotation1 = JSONFileIO.Load<RotationModel>(rotation1FilePath);

            employeeListBox.ItemsSource = employeeList.EmployeeList;
            rotation1Name.Text = rotation1.RotationName;
            rotation1ListBox.ItemsSource = rotation1.Rotation;
        }

        private void EditEmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            EditEmployees editEmployees = new(this);

            editEmployees.Show();
        }

        private void EditRotation1Button_Click(object sender, RoutedEventArgs e)
        {
            EditRotation editRotation = new(this);

            editRotation.Show();
        }
    }
}
