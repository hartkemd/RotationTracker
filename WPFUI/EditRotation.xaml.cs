using JSONFileIOLibrary;
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
        MainWindow _parent;

        public EditRotation(MainWindow parent)
        {
            InitializeComponent();

            _parent = parent;

            rotationNameTextBlock.Text = _parent.rotation1.RotationName;
            employeeListBox.ItemsSource = _parent.rotation1.Rotation;
            rotationNameTextBox.Text = _parent.rotation1.RotationName;
        }

        private void SaveRotation()
        {
            _parent.rotation1.Save(_parent.rotation1FilePath);
        }

        private void RefreshListBoxes()
        {
            employeeListBox.ItemsSource = null;
            _parent.rotation1ListBox.ItemsSource = null;

            employeeListBox.ItemsSource = _parent.rotation1.Rotation;
            _parent.rotation1ListBox.ItemsSource = _parent.rotation1.Rotation;
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = employeeListBox.SelectedIndex;
            
            if (selectedIndex > 0)
            {
                _parent.rotation1.Rotation.Insert(selectedIndex - 1, employeeListBox.Items[selectedIndex].ToString());
                _parent.rotation1.Rotation.RemoveAt(selectedIndex + 1);

                RefreshListBoxes();
                employeeListBox.SelectedIndex = selectedIndex - 1;
                SaveRotation();
            }
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = employeeListBox.SelectedIndex;

            if (selectedIndex < employeeListBox.Items.Count - 1 && selectedIndex != -1)
            {
                _parent.rotation1.Rotation.Insert(selectedIndex + 2, employeeListBox.Items[selectedIndex].ToString());
                _parent.rotation1.Rotation.RemoveAt(selectedIndex);

                RefreshListBoxes();
                employeeListBox.SelectedIndex = selectedIndex + 1;
                SaveRotation();
            }
        }

        private void CopyEmployeesToRotation_Click(object sender, RoutedEventArgs e)
        {
            _parent.rotation1.Rotation = _parent.employeeList.EmployeeList;

            RefreshListBoxes();
            SaveRotation();
        }

        private void RenameRotationButton_Click(object sender, RoutedEventArgs e)
        {
            _parent.rotation1.RotationName = rotationNameTextBox.Text;
            rotationNameTextBlock.Text = $"{_parent.rotation1.RotationName} Rotation";
            _parent.rotation1Name.Text = $"{_parent.rotation1.RotationName} Rotation";

            SaveRotation();
        }

        private void AdvanceButton_Click(object sender, RoutedEventArgs e)
        {
            _parent.rotation1.AdvanceRotation();

            RefreshListBoxes();
            SaveRotation();
        }
    }
}
