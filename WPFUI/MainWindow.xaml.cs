using JSONFileIOLibrary;
using RotationLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public EmployeeListModel employees = new();
        public RotationModel rotation1 = new();
        public RotationModel rotation2 = new();

        public MainWindow()
        {
            InitializeComponent();

            SetObjectFilePaths();
            LoadObjectData();

            SetRotationNames();
            SetDataSourcesOfControls();

            AdvanceRotationsIfDateTimeHasPassed();
        }

        private void SetObjectFilePaths()
        {
            employees.FilePath = "EmployeeList.json";
            rotation1.FilePath = "Rotation1.json";
            rotation2.FilePath = "Rotation2.json";
        }

        private void LoadObjectData()
        {
            employees = employees.Load(employees.FilePath);
            rotation1 = rotation1.Load(rotation1.FilePath);
            rotation2 = rotation2.Load(rotation2.FilePath);
        }

        private void SetRotationNames()
        {
            SetRotationName(rotation1, "First");
            SetRotationName(rotation2, "Second");
        }

        private static void SetRotationName(RotationModel rotation, string name)
        {
            if (rotation.RotationName == null)
            {
                rotation.RotationName = name;
            }
        }

        private void SetDataSourcesOfControls()
        {
            employeeListBox.ItemsSource = employees.EmployeeList;

            rotation1Name.Content = $"{rotation1.RotationName} Rotation:";
            rotation1ListBox.ItemsSource = rotation1.Rotation;
            rotation1NextUpName.Text = rotation1.NextUp;

            rotation2Name.Content = $"{rotation2.RotationName} Rotation:";
            rotation2ListBox.ItemsSource = rotation2.Rotation;
            rotation2NextUpName.Text = rotation2.NextUp;
        }

        private void AdvanceRotationsIfDateTimeHasPassed()
        {
            AdvanceRotationIfDateTimeHasPassed(rotation1, rotation1NextUpName, rotation1ListBox);
        }

        private static void AdvanceRotationIfDateTimeHasPassed(RotationModel rotation, TextBlock textBlock, ListBox listBox)
        {
            // work on this

            DateTime now = DateTime.Now;

            if (now > rotation.DateTimeRotationAdvances)
            {
                rotation.AdvanceRotation();
                rotation.DateTimeRotationAdvances = now.AddDays(7);
                textBlock.Text = rotation.NextUp;
                RefreshRotationListBox(listBox, rotation);
                SaveRotation(rotation);
            }
        }

        private static void SaveRotation(RotationModel rotation)
        {
            rotation.Save(rotation.FilePath);
        }

        private static void RefreshRotationListBox(ListBox listBox, RotationModel rotation)
        {
            listBox.ItemsSource = null;
            listBox.ItemsSource = rotation.Rotation;
        }

        private static void AdvanceRotation(RotationModel rotation, TextBlock textBlock, ListBox listBox)
        {
            rotation.AdvanceRotation();
            textBlock.Text = rotation.NextUp;

            RefreshRotationListBox(listBox, rotation);
            SaveRotation(rotation);
        }

        private void EditRotation(RotationModel rotation, ListBox listBox, Label label)
        {
            EditRotation editRotation = new(this, rotation, listBox, label);
            editRotation.ShowDialog();
        }

        private void EditEmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            EditEmployees editEmployees = new(this);
            editEmployees.ShowDialog();
        }

        private void AdvanceRotation1Button_Click(object sender, RoutedEventArgs e)
        {
            AdvanceRotation(rotation1, rotation1NextUpName, rotation1ListBox);
        }

        private void EditRotation1Button_Click(object sender, RoutedEventArgs e)
        {
            EditRotation(rotation1, rotation1ListBox, rotation1Name);
        }

        private void AdvanceRotation2Button_Click(object sender, RoutedEventArgs e)
        {
            AdvanceRotation(rotation2, rotation2NextUpName, rotation2ListBox);
        }

        private void EditRotation2Button_Click(object sender, RoutedEventArgs e)
        {
            EditRotation(rotation2, rotation2ListBox, rotation2Name);
        }
    }
}
