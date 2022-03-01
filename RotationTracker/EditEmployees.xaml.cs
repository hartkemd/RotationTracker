using DataAccessLibrary.Models;
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
        private EmployeeModel _employeeToRemove;

        public EditEmployees(MainWindow parent)
        {
            InitializeComponent();

            _parent = parent;

            employeeListBox.ItemsSource = _parent.employees;
        }

        private void RemoveEmployeeFromAllRotationsAndSave()
        {
            foreach (var rotationUIModel in _parent.rotationUIModels)
            {
                rotationUIModel.FullRotationModel.RotationOfEmployees.Remove(_employeeToRemove);
                //rotationUIModel.RotationListBox.RefreshContents(rotationUIModel.FullRotationModel.RotationOfEmployees);
                rotationUIModel.CurrentEmployeeTextBlock.Text = rotationUIModel.FullRotationModel.CurrentEmployeeName;
            }
            
            //_parent.rotation1.SaveToJSON(_parent.rotation1.FilePath, _parent.rotation1.FileName);
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            string employeeName = employeeNameTextBox.Text;

            _parent.CreateEmployeeInDBAsync(employeeName);

            _parent.ReadEmployeesFromDBAsync();

            employeeNameTextBox.Clear();

            employeeListBox.RefreshContents(_parent.employees);
            _parent.employeeListBox.RefreshContents(_parent.employees);
        }

        private void RemoveEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            int index = employeeListBox.SelectedIndex;

            if (index != -1)
            {
                MessageBoxResult messageBoxResult =
                WPFHelper.ShowYesNoExclamationMessageBox("This will remove the employee from this list and all rotations. Are you sure?",
                "Remove?");
                
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    EmployeeModel employeeToDelete = (EmployeeModel)employeeListBox.SelectedItem;
                    _parent.DeleteEmployeeFromDBAsync(employeeToDelete.Id);
                    _parent.employees.RemoveAt(index);
                    employeeListBox.RefreshContents(_parent.employees);
                    _parent.employeeListBox.RefreshContents(_parent.employees);

                    RemoveEmployeeFromAllRotationsAndSave();
                }
            }
        }
    }
}
