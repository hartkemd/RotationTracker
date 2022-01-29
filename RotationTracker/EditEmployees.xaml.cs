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

            employeeListBox.ItemsSource = _parent.employees;
        }

        private void RemoveEmployeeFromAllRotationsAndSave()
        {
            foreach (var rotationUIModel in _parent.rotationUIModels)
            {
                rotationUIModel.RotationModel.Rotation.Remove(_employeeToRemove);
                rotationUIModel.RotationListBox.RefreshContents(rotationUIModel.RotationModel.Rotation);
                rotationUIModel.CurrentEmployeeTextBlock.Text = rotationUIModel.RotationModel.CurrentEmployee;
            }
            
            //_parent.rotation1.SaveToJSON(_parent.rotation1.FilePath, _parent.rotation1.FileName);
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            _parent.employees.Add(employeeNameTextBox.Text);
            _parent.employees.Sort();

            employeeNameTextBox.Clear();

            employeeListBox.RefreshContents(_parent.employees);
            _parent.employeeListBox.RefreshContents(_parent.employees);

            //_parent.employees.SaveToJSON(_parent.employees.FilePath, _parent.employees.FileName);
        }

        private void RemoveEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult =
                WPFHelper.ShowYesNoExclamationMessageBox("This will remove the employee from this list and all rotations. Are you sure?",
                "Remove?");
            
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                _employeeToRemove = employeeListBox.SelectedItem.ToString();
                _parent.employees.RemoveAt(employeeListBox.SelectedIndex);
                //_parent.employees.SaveToJSON(_parent.employees.FilePath, _parent.employees.FileName);

                employeeListBox.RefreshContents(_parent.employees);
                _parent.employeeListBox.RefreshContents(_parent.employees);

                RemoveEmployeeFromAllRotationsAndSave();
            }
        }
    }
}
