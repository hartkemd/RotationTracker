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
using System.Windows.Shapes;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for EditEmployees.xaml
    /// </summary>
    public partial class EditEmployees : Window
    {
        private MainWindow _parent;
        private string _employeeToRemove;

        public EditEmployees(MainWindow parent)
        {
            InitializeComponent();

            _parent = parent;

            employeeListBox.ItemsSource = _parent.employees.EmployeeList;
        }

        private void SaveEmployeeList()
        {
            _parent.employees.Save(_parent.employees.FilePath);
        }

        private void RefreshListBoxOnThisWindow()
        {
            employeeListBox.ItemsSource = null;
            employeeListBox.ItemsSource = _parent.employees.EmployeeList;
        }
        
        private void RefreshListBoxOnParentWindow()
        {
            _parent.employeeListBox.ItemsSource = null;
            _parent.employeeListBox.ItemsSource = _parent.employees.EmployeeList;
        }

        private void RemoveEmployeeFromAllRotationsAndSave()
        {
            _parent.rotation1.Rotation.Remove(_employeeToRemove);

            _parent.rotation1ListBox.ItemsSource = null;
            _parent.rotation1ListBox.ItemsSource = _parent.rotation1.Rotation;

            _parent.rotation1.Save(_parent.rotation1.FilePath);
            _parent.rotation1CurrentEmployeeTextBlock.Text = _parent.rotation1.CurrentEmployee;

            _parent.rotation2.Rotation.Remove(_employeeToRemove);

            _parent.rotation2ListBox.ItemsSource = null;
            _parent.rotation2ListBox.ItemsSource = _parent.rotation2.Rotation;

            _parent.rotation2.Save(_parent.rotation2.FilePath);
            _parent.rotation2CurrentEmployeeTextBlock.Text = _parent.rotation2.CurrentEmployee;
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            _parent.employees.EmployeeList.Add(employeeNameTextBox.Text);
            _parent.employees.EmployeeList.Sort();

            employeeNameTextBox.Clear();

            RefreshListBoxOnThisWindow();
            RefreshListBoxOnParentWindow();

            _parent.employees.EmployeeList = _parent.employees.EmployeeList;
            SaveEmployeeList();
        }

        private void RemoveEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult =
                MessageBox.Show("This will remove the employee from this list and all rotations. Are you sure?",
                "Remove?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                _employeeToRemove = employeeListBox.SelectedItem.ToString();
                _parent.employees.EmployeeList.RemoveAt(employeeListBox.SelectedIndex);
                _parent.employees.Save(_parent.employees.FilePath);

                RefreshListBoxOnThisWindow();
                RefreshListBoxOnParentWindow();

                RemoveEmployeeFromAllRotationsAndSave();
            }
        }
    }
}
