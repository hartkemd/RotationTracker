using JSONFileIOLibrary;
using RotationLibrary;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using WPFHelperLibrary;

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
        private string notificationMessage;

        public MainWindow()
        {
            InitializeComponent();

            SetObjectFilePaths();
            LoadObjectData();
            SetObjectFilePaths();

            SetDataSourcesOfControls();

            AdvanceRotationsIfDateTimeHasPassed();

            DisplayNotificationsAsync();
            CreateTimer();
        }

        private void CreateTimer()
        {
            Timer refreshAppTimer = new();
            refreshAppTimer.Interval = 15 * 60 * 1000; // 15 minutes
            refreshAppTimer.Elapsed += new ElapsedEventHandler(RefreshAppTimer_Elapsed);
            refreshAppTimer.Start();
        }

        private void RefreshAppTimer_Elapsed(object sender, EventArgs e)
        {
            AdvanceRotationsIfDateTimeHasPassed();
        }

        private async void DisplayNotificationsAsync()
        {
            if (notificationMessage != "")
            {
                notificationTextBlock.Text = notificationMessage;
                notificationStackPanel.Visibility = Visibility.Visible;
                await Task.Delay(5000);
                notificationStackPanel.Visibility = Visibility.Collapsed;
            }
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

        private void SetDataSourcesOfControls()
        {
            employeeListBox.ItemsSource = employees.EmployeeList;

            rotation1.SetRotationControlsOnMainWindow(rotation1NameLabel, rotation1ListBox, rotation1CurrentEmployeeTextBlock);
            rotation2.SetRotationControlsOnMainWindow(rotation2NameLabel, rotation2ListBox, rotation2CurrentEmployeeTextBlock);
        }

        private void AdvanceRotationsIfDateTimeHasPassed()
        {
            AdvanceRotationIfDateTimeHasPassed(rotation1, rotation1CurrentEmployeeTextBlock, rotation1ListBox);
            AdvanceRotationIfDateTimeHasPassed(rotation2, rotation2CurrentEmployeeTextBlock, rotation2ListBox);
        }

        private void AdvanceRotationIfDateTimeHasPassed(RotationModel rotation, TextBlock textBlock, ListBox listBox)
        {
            if (rotation.NextDateTimeRotationAdvances != DateTime.MinValue)
            {
                DateTime now = DateTime.Now;

                if (now > rotation.NextDateTimeRotationAdvances)
                {
                    rotation.AdvanceRotation();
                    rotation.SetNextDateTimeRotationAdvances();
                    if (rotation.Rotation.Count > 0)
                    {
                        notificationMessage += $"{rotation.Rotation.Last()} took their turn for {rotation.RotationName} Rotation.\n";
                    }
                    textBlock.Text = rotation.CurrentEmployee;
                    listBox.RefreshContents(rotation.Rotation);
                    SaveRotation(rotation);
                }
            }
        }

        private static void SaveRotation(RotationModel rotation)
        {
            rotation.Save(rotation.FilePath);
        }

        private static void AdvanceRotationAndRefreshControls(RotationModel rotation, TextBlock textBlock, ListBox listBox)
        {
            rotation.AdvanceRotation();
            textBlock.Text = rotation.CurrentEmployee;

            listBox.RefreshContents(rotation.Rotation);
            SaveRotation(rotation);
        }

        private void EditRotation(RotationModel rotation, ListBox listBox, Label label, TextBlock textBlock)
        {
            EditRotation editRotation = new(this, rotation, listBox, label, textBlock);
            editRotation.ShowDialog();
        }

        private void EditEmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            EditEmployees editEmployees = new(this);
            editEmployees.ShowDialog();
        }

        private void AdvanceRotation1Button_Click(object sender, RoutedEventArgs e)
        {
            AdvanceRotationAndRefreshControls(rotation1, rotation1CurrentEmployeeTextBlock, rotation1ListBox);
        }

        private void EditRotation1Button_Click(object sender, RoutedEventArgs e)
        {
            EditRotation(rotation1, rotation1ListBox, rotation1NameLabel, rotation1CurrentEmployeeTextBlock);
        }

        private void AdvanceRotation2Button_Click(object sender, RoutedEventArgs e)
        {
            AdvanceRotationAndRefreshControls(rotation2, rotation2CurrentEmployeeTextBlock, rotation2ListBox);
        }

        private void EditRotation2Button_Click(object sender, RoutedEventArgs e)
        {
            EditRotation(rotation2, rotation2ListBox, rotation2NameLabel, rotation2CurrentEmployeeTextBlock);
        }
    }
}
