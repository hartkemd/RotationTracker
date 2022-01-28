using RotationLibrary.Models;
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

            rotationComboBox.ItemsSource = _parentWindow.rotations;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (rotationComboBox.SelectedIndex != -1)
            {
                _parentWindow.rotations.Remove(rotationComboBox.SelectedItem as RotationModel);
                _parentWindow.rotationsWrapPanel.Children.RemoveAt(rotationComboBox.SelectedIndex);
                rotationComboBox.Items.Refresh();
            }
        }
    }
}
