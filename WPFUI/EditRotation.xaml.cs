using JSONFileIOLibrary;
using RotationLibrary;
using System;
using System.Windows;
using System.Windows.Controls;
using WPFHelperLibrary;

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
        private TextBlock _textBlock;

        public EditRotation(MainWindow parentWindow, RotationModel rotation, ListBox listBox, Label label, TextBlock textBlock)
        {
            InitializeComponent();

            _parent = parentWindow;
            _rotation = rotation;
            _listBox = listBox;
            _label = label;
            _textBlock = textBlock;

            rotationNameLabel.Content = $"{rotation.RotationName} Rotation:";
            employeeListBox.ItemsSource = rotation.Rotation;
            rotationNameTextBox.Text = rotation.RotationName;
            if (_rotation.NextDateTimeRotationAdvances == DateTime.MinValue)
            {
                nextDateRotationAdvancesDatePicker.SelectedDate = DateTime.Today;
            }
            else if (_rotation.NextDateTimeRotationAdvances != DateTime.MinValue)
            {
                nextDateRotationAdvancesDatePicker.SelectedDate = _rotation.NextDateTimeRotationAdvances;
            }
            hourRotationAdvancesTextBox.Text = _rotation.NextDateTimeRotationAdvances.Hour.ToString();
        }

        private void RefreshListBoxes()
        {
            employeeListBox.RefreshContents(_rotation.Rotation);
            _listBox.RefreshContents(_rotation.Rotation);
        }

        private void SetRotationRecurrence()
        {
            if (weeklyRadioButton.IsChecked == true)
            {
                _rotation.RotationRecurrence = RecurrenceInterval.Weekly;
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

        private bool SetNextDateTimeRotationAdvances()
        {
            bool dateTimeSetSuccessfully = true;

            if (nextDateRotationAdvancesDatePicker.SelectedDate.HasValue)
            {
                _rotation.NextDateTimeRotationAdvances = nextDateRotationAdvancesDatePicker.SelectedDate.Value;
                if (string.IsNullOrWhiteSpace(hourRotationAdvancesTextBox.Text) == false)
                {
                    bool isValidInt = int.TryParse(hourRotationAdvancesTextBox.Text, out int hoursToAdd);
                    if (isValidInt && hoursToAdd >= 0 && hoursToAdd <= 23)
                    {
                        _rotation.NextDateTimeRotationAdvances = _rotation.NextDateTimeRotationAdvances.AddHours(hoursToAdd);
                    }
                    else
                    {
                        WPFHelper.TextBoxInputWasInvalid("That was not a valid number of hours.", hourRotationAdvancesTextBox);
                        dateTimeSetSuccessfully = false;
                    }
                }
            }
            else
            {
                MessageBox.Show("No date was selected.");
                dateTimeSetSuccessfully = false;
            }

            return dateTimeSetSuccessfully;
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
            employeeListBox.RefreshContents(_rotation.Rotation);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _listBox.RefreshContents(_rotation.Rotation);

            _rotation.RotationName = rotationNameTextBox.Text;
            _label.Content = $"{_rotation.RotationName} Rotation:";

            _textBlock.Text = _rotation.CurrentEmployee;

            SetRotationRecurrence();

            bool keepGoing = SetNextDateTimeRotationAdvances();
            if (keepGoing)
            {
                _rotation.Save(_rotation.FilePath, _rotation.FileName);
                Close();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult =
                WPFHelper.ShowYesNoExclamationMessageBox("This will delete the rotation. Are you sure?",
                "Delete?");

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                _rotation.Clear();
                _rotation.Save(_rotation.FilePath, _rotation.FileName);
                _listBox.RefreshContents(_rotation.Rotation);
                _label.Content = "Rotation:";
                _textBlock.Text = "";
                Close();
            }
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            _rotation.Rotation.Add(employeeNameTextBox.Text);

            employeeNameTextBox.Clear();

            employeeListBox.RefreshContents(_rotation.Rotation);
        }

        private void RemoveEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (employeeListBox.SelectedIndex != -1)
            {
                _rotation.Rotation.Remove(employeeListBox.SelectedItem.ToString());
                employeeListBox.RefreshContents(_rotation.Rotation);
            }
        }
    }
}
