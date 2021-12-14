using JSONFileIOLibrary;
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
using System.Windows.Shapes;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for EditEmployees.xaml
    /// </summary>
    public partial class EditEmployees : Window
    {
        MainWindow _parent;

        public EditEmployees(MainWindow parent)
        {
            InitializeComponent();

            _parent = parent;

            employeeListBox.ItemsSource = _parent.employeeList.EmployeeList;
        }

        private void SaveEmployeeList()
        {
            _parent.employeeList.Save(_parent.employeeListFilePath);
        }

        private void RefreshListBoxes()
        {
            employeeListBox.ItemsSource = null;
            _parent.employeeListBox.ItemsSource = null;

            employeeListBox.ItemsSource = _parent.employeeList.EmployeeList;
            _parent.employeeListBox.ItemsSource = _parent.employeeList.EmployeeList;
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            _parent.employeeList.EmployeeList.Add(employeeNameTextBox.Text);
            _parent.employeeList.EmployeeList.Sort();

            SaveEmployeeList();

            employeeNameTextBox.Clear();

            RefreshListBoxes();
        }

        private void RemoveEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            _parent.employeeList.EmployeeList.RemoveAt(employeeListBox.SelectedIndex);

            SaveEmployeeList();

            RefreshListBoxes();
        }
    }
}
