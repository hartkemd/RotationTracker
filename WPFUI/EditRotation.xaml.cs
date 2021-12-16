using JSONFileIOLibrary;
using RotationLibrary;
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

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for EditRotation.xaml
    /// </summary>
    public partial class EditRotation : Window
    {
        private MainWindow _parent;
        private RotationModel _rotation;
        private ListBox _listBox;
        private Label _label;

        public EditRotation(MainWindow parentWindow, RotationModel rotation, ListBox listBox, Label label)
        {
            InitializeComponent();

            _parent = parentWindow;
            _rotation = rotation;
            _listBox = listBox;
            _label = label;

            rotationNameLabel.Content = $"{rotation.RotationName} Rotation:";
            employeeListBox.ItemsSource = rotation.Rotation;
            rotationNameTextBox.Text = rotation.RotationName;
        }

        private void RefreshListBoxes()
        {
            employeeListBox.ItemsSource = null;
            _listBox.ItemsSource = null;

            employeeListBox.ItemsSource = _rotation.Rotation;
            _listBox.ItemsSource = _rotation.Rotation;
        }

        private void SetRotationRecurrence()
        {
            if (weeklyRadioButton.IsChecked == true)
            {
                _rotation.RotationRecurrence = RecurrenceInterval.Weekly;
                SetRecurrenceDayOfWeek();
            }
            else if (monthlyRadioButton.IsChecked == true)
            {
                _rotation.RotationRecurrence = RecurrenceInterval.Monthly;
            }
            else if (bimonthlyRadioButton.IsChecked == true)
            {
                _rotation.RotationRecurrence = RecurrenceInterval.Bimonthly;
            }
        }

        private void SetRecurrenceDayOfWeek()
        {
            if (sundayRadioButton.IsChecked == true)
            {
                _rotation.RotationRecurrenceDayOfWeek = DayOfWeek.Sunday;
            }
            else if (mondayRadioButton.IsChecked == true)
            {
                _rotation.RotationRecurrenceDayOfWeek = DayOfWeek.Monday;
            }
            else if (tuesdayRadioButton.IsChecked == true)
            {
                _rotation.RotationRecurrenceDayOfWeek = DayOfWeek.Tuesday;
            }
            else if (wednesdayRadioButton.IsChecked == true)
            {
                _rotation.RotationRecurrenceDayOfWeek = DayOfWeek.Wednesday;
            }
            else if (thursdayRadioButton.IsChecked == true)
            {
                _rotation.RotationRecurrenceDayOfWeek = DayOfWeek.Thursday;
            }
            else if (fridayRadioButton.IsChecked == true)
            {
                _rotation.RotationRecurrenceDayOfWeek = DayOfWeek.Friday;
            }
            else if (saturdayRadioButton.IsChecked == true)
            {
                _rotation.RotationRecurrenceDayOfWeek = DayOfWeek.Saturday;
            }
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = employeeListBox.SelectedIndex;

            if (selectedIndex > 0)
            {
                _rotation.Rotation.Insert(selectedIndex - 1, employeeListBox.Items[selectedIndex].ToString());
                _rotation.Rotation.RemoveAt(selectedIndex + 1);

                RefreshListBoxes();
                employeeListBox.SelectedIndex = selectedIndex - 1;
            }
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = employeeListBox.SelectedIndex;

            if (selectedIndex < employeeListBox.Items.Count - 1 && selectedIndex != -1)
            {
                _rotation.Rotation.Insert(selectedIndex + 2, employeeListBox.Items[selectedIndex].ToString());
                _rotation.Rotation.RemoveAt(selectedIndex);

                RefreshListBoxes();
                employeeListBox.SelectedIndex = selectedIndex + 1;
            }
        }

        private void CopyEmployeesToRotation_Click(object sender, RoutedEventArgs e)
        {
            _rotation.Rotation = _parent.employees.EmployeeList;

            RefreshListBoxes();
        }

        private void RenameRotationButton_Click(object sender, RoutedEventArgs e)
        {
            _rotation.RotationName = rotationNameTextBox.Text;
            rotationNameLabel.Content = $"{_rotation.RotationName} Rotation:";
            _label.Content = $"{_rotation.RotationName} Rotation:";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SetRotationRecurrence();
            _rotation.Save(_rotation.FilePath);
            Close();
        }
    }
}
