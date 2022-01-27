using JSONFileIOLibrary;
using System.Windows;
using WPFHelperLibrary;

namespace RotationTracker
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

        private void RemoveEmployeeFromAllRotationsAndSave()
        {
            _parent.rotation1.Rotation.Remove(_employeeToRemove);

            _parent.rotation1ListBox.RefreshContents(_parent.rotation1.Rotation);
            _parent.rotation1.SaveToJSON(_parent.rotation1.FilePath, _parent.rotation1.FileName);

            _parent.rotation1CurrentEmployeeTextBlock.Text = _parent.rotation1.CurrentEmployee;
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            _parent.employees.EmployeeList.Add(employeeNameTextBox.Text);
            _parent.employees.EmployeeList.Sort();

            employeeNameTextBox.Clear();

            employeeListBox.RefreshContents(_parent.employees.EmployeeList);
            _parent.employeeListBox.RefreshContents(_parent.employees.EmployeeList);

            _parent.employees.EmployeeList = _parent.employees.EmployeeList;

            _parent.employees.SaveToJSON(_parent.employees.FilePath, _parent.employees.FileName);
        }

        private void RemoveEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult =
                WPFHelper.ShowYesNoExclamationMessageBox("This will remove the employee from this list and all rotations. Are you sure?",
                "Remove?");
            
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                _employeeToRemove = employeeListBox.SelectedItem.ToString();
                _parent.employees.EmployeeList.RemoveAt(employeeListBox.SelectedIndex);
                _parent.employees.SaveToJSON(_parent.employees.FilePath, _parent.employees.FileName);

                employeeListBox.RefreshContents(_parent.employees.EmployeeList);
                _parent.employeeListBox.RefreshContents(_parent.employees.EmployeeList);

                RemoveEmployeeFromAllRotationsAndSave();
            }
        }
    }
}
