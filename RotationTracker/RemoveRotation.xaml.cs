using DataAccessLibrary.Models;
using System.Windows;

namespace RotationTracker
{
    /// <summary>
    /// Interaction logic for RemoveRotation.xaml
    /// </summary>
    public partial class RemoveRotation : Window
    {
        private readonly MainWindow _parentWindow;

        public RemoveRotation(MainWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;

            SetRotationComboBox();
        }

        private void SetRotationComboBox()
        {
            rotationComboBox.ItemsSource = _parentWindow.rotations;

            if (rotationComboBox.Items.Count > 0)
            {
                rotationComboBox.SelectedIndex = 0;
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = rotationComboBox.SelectedIndex;

            if (selectedIndex != -1)
            {
                FullRotationModel fullRotationModel = (FullRotationModel)rotationComboBox.SelectedItem;
                _parentWindow.rotations.Remove(fullRotationModel);
                _parentWindow.rotationsWrapPanel.Children.RemoveAt(selectedIndex);
                rotationComboBox.Items.Refresh();
                _parentWindow.DeleteRotationFromDBAsync(fullRotationModel.BasicInfo.Id);
            }
        }
    }
}
