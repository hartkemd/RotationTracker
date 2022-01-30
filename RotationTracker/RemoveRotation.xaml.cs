using DataAccessLibrary.Models;
using RotationLibrary.Models;
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
            if (rotationComboBox.SelectedIndex != -1)
            {
                _parentWindow.rotations.Remove(rotationComboBox.SelectedItem as FullRotationModel);
                _parentWindow.rotationsWrapPanel.Children.RemoveAt(rotationComboBox.SelectedIndex);
                rotationComboBox.Items.Refresh();
            }
        }
    }
}
