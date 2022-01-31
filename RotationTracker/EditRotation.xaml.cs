using DataAccessLibrary.Models;
using RotationLibrary;
using RotationTracker.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WPFHelperLibrary;

namespace RotationTracker
{
    /// <summary>
    /// Interaction logic for EditRotation.xaml
    /// </summary>
    public partial class EditRotation : Window
    {
        private MainWindow _parentWindow;
        private RotationUIModel _rotationUIModel;
        private FullRotationModel _rotation;
        private ListBox _listBox;
        private Label _label;
        private TextBlock _textBlock;

        public EditRotation(MainWindow parentWindow, RotationUIModel rotationUIModel)
        {
            InitializeComponent();

            _parentWindow = parentWindow;
            _rotationUIModel = rotationUIModel;
            _rotation = _rotationUIModel.FullRotationModel;
            _listBox = _rotationUIModel.RotationListBox;
            _label = _rotationUIModel.RotationNameLabel;
            _textBlock = _rotationUIModel.CurrentEmployeeTextBlock;

            PopulateControls();
        }

        private void PopulateControls()
        {
            rotationNameLabel.Content = $"{_rotation.BasicInfo.RotationName}:";
            employeeListBox.ItemsSource = _rotation.RotationOfEmployees;
            rotationNameTextBox.Text = _rotation.BasicInfo.RotationName;
            notesTextBox.Text = _rotation.BasicInfo.Notes;
            GetRotationRecurrence();

            if (_rotation.BasicInfo.NextDateTimeRotationAdvances == DateTime.MinValue)
            {
                nextDateRotationAdvancesDatePicker.SelectedDate = DateTime.Today;
            }
            else
            {
                nextDateRotationAdvancesDatePicker.SelectedDate = _rotation.BasicInfo.NextDateTimeRotationAdvances;
            }

            hourRotationAdvancesTextBox.Text = _rotation.BasicInfo.NextDateTimeRotationAdvances.Hour.ToString();
        }

        private void GetRotationRecurrence()
        {
            switch (_rotation.BasicInfo.RotationRecurrence)
            {
                case RecurrenceInterval.Weekly:
                    weeklyRadioButton.IsChecked = true;
                    break;
                case RecurrenceInterval.Monthly:
                    monthlyRadioButton.IsChecked = true;
                    break;
                case RecurrenceInterval.Bimonthly:
                    bimonthlyRadioButton.IsChecked = true;
                    break;
            }
        }

        private void RefreshListBoxes()
        {
            employeeListBox.RefreshContents(_rotation.RotationOfEmployees);
            _listBox.RefreshContents(_rotation.RotationOfEmployees);
        }

        private void SetRotationRecurrence()
        {
            if (weeklyRadioButton.IsChecked == true)
            {
                _rotation.BasicInfo.RotationRecurrence = RecurrenceInterval.Weekly;
            }
            else if (monthlyRadioButton.IsChecked == true)
            {
                _rotation.BasicInfo.RotationRecurrence = RecurrenceInterval.Monthly;
            }
            else if (bimonthlyRadioButton.IsChecked == true)
            {
                _rotation.BasicInfo.RotationRecurrence = RecurrenceInterval.Bimonthly;
            }
        }

        private bool SetNextDateTimeRotationAdvances()
        {
            bool dateTimeSetSuccessfully = true;

            if (nextDateRotationAdvancesDatePicker.SelectedDate.HasValue)
            {
                _rotation.BasicInfo.NextDateTimeRotationAdvances = nextDateRotationAdvancesDatePicker.SelectedDate.Value;
                if (string.IsNullOrWhiteSpace(hourRotationAdvancesTextBox.Text) == false)
                {
                    bool isValidInt = int.TryParse(hourRotationAdvancesTextBox.Text, out int hoursToAdd);
                    if (isValidInt && hoursToAdd >= 0 && hoursToAdd <= 23)
                    {
                        _rotation.BasicInfo.NextDateTimeRotationAdvances = _rotation.BasicInfo.NextDateTimeRotationAdvances.AddHours(hoursToAdd);
                    }
                    else
                    {
                        WPFHelper.ShowErrorMessageBoxAndResetTextBox("That was not a valid number of hours.",
                            hourRotationAdvancesTextBox);
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
                _rotation.RotationOfEmployees.Insert(selectedIndex - 1, (EmployeeModel)employeeListBox.Items[selectedIndex]);
                _rotation.RotationOfEmployees.RemoveAt(selectedIndex + 1);

                RefreshListBoxes();
                employeeListBox.SelectedIndex = selectedIndex - 1;
            }
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = employeeListBox.SelectedIndex;

            if (selectedIndex < employeeListBox.Items.Count - 1 && selectedIndex != -1)
            {
                _rotation.RotationOfEmployees.Insert(selectedIndex + 2, (EmployeeModel)employeeListBox.Items[selectedIndex]);
                _rotation.RotationOfEmployees.RemoveAt(selectedIndex);

                RefreshListBoxes();
                employeeListBox.SelectedIndex = selectedIndex + 1;
            }
        }

        private void RemoveEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (employeeListBox.SelectedIndex != -1)
            {
                _rotation.RotationOfEmployees.Remove((EmployeeModel)employeeListBox.SelectedItem);
                employeeListBox.RefreshContents(_rotation.RotationOfEmployees);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _listBox.RefreshContents(_rotation.RotationOfEmployees);

            _rotation.BasicInfo.RotationName = rotationNameTextBox.Text;
            _label.Content = $"{_rotation.BasicInfo.RotationName}:";

            _textBlock.Text = $"Currently Up: {_rotation.CurrentEmployee}";

            _rotation.BasicInfo.Notes = notesTextBox.Text;
            _rotationUIModel.RotationNotesTextBox.Text = _rotation.BasicInfo.Notes;

            SetRotationRecurrence();

            bool keepGoing = SetNextDateTimeRotationAdvances();
            if (keepGoing == true)
            {
                _parentWindow.UpdateRotationBasicInfoInDB(_rotation.BasicInfo);
                _parentWindow.RecreateRotationInDB(_rotation);
                Close();
            }
        }
    }
}
