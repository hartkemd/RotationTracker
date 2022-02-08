using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using RotationLibrary;
using RotationTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using WPFHelperLibrary;

namespace RotationTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ISqliteData _db;
        public List<EmployeeModel> employees = new ();
        public List<FullRotationModel> rotations = new ();
        public List<RotationUIModel> rotationUIModels = new ();
        private List<string> admins = new();
        private string currentUser;
        private bool currentUserIsAdmin = false;
        private string notificationMessage;

        public MainWindow(ISqliteData db)
        {
            InitializeComponent();
            _db = db;

            ReadEmployeesFromDB();
            employeeListBox.ItemsSource = employees;
            ReadRotationsFromDB();
            LoadRotationsIntoUI();

            GetCurrentUser();
            DisplayCurrentUser();
            ReadAdminsFromDB();
            CheckIfCurrentUserIsAdmin();
            ShowControlsIfCurrentUserIsAdmin();

            AdvanceRotationsIfDateTimeHasPassed();

            DisplayNotificationsAsync();
            CreateTimer();
        }

        public void ReadEmployeesFromDB()
        {
            employees = _db.GetAllEmployees();
        }

        public void ReadRotationsFromDB()
        {
            rotations = _db.GetAllRotations();
        }

        private void LoadRotationsIntoUI()
        {
            foreach (var rotation in rotations)
            {
                LoadRotationIntoUI(rotation);
            }
        }

        private void LoadRotationIntoUI(FullRotationModel rotation)
        {
            RotationUIModel rotationUIModel = new();
            rotationUIModel.FullRotationModel = rotation;

            CreateRotationInUI(rotationUIModel, rotation);
        }

        public void CreateEmployeeInDB(string employeeName)
        {
            _db.CreateEmployee(employeeName);
        }

        public void DeleteEmployeeFromDB(int id)
        {
            _db.DeleteEmployee(id);
        }

        private void CreateRotationInDB(FullRotationModel fullRotation)
        {
            _db.CreateRotation(fullRotation);
        }

        public void RecreateRotationInDB(FullRotationModel fullRotation)
        {
            _db.RecreateRotationOfEmployees(fullRotation);
        }

        public void UpdateRotationBasicInfoInDB(BasicRotationModel basicRotation)
        {
            _db.UpdateRotationBasicInfo(basicRotation);
        }

        private void AdvanceRotationInDB(FullRotationModel fullRotation)
        {
            _db.AdvanceRotation(fullRotation);
        }

        public void DeleteRotationFromDB(int id)
        {
            _db.DeleteRotation(id);
        }

        private void CreateRotationInUI(RotationUIModel rotationUIModel, FullRotationModel rotation)
        {
            GroupBox groupBox = new GroupBox();
            groupBox.Margin = new Thickness(5);
            Label label = new Label();
            label.Content = $"{rotation.BasicInfo.RotationName}:";
            rotationUIModel.RotationNameLabel = label;
            groupBox.Header = label;

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            ListBox listBox = new ListBox();
            listBox.Margin = new Thickness(5, 0, 5, 0);
            listBox.ItemsSource = rotation.RotationOfEmployees;
            listBox.DisplayMemberPath = "FullName";
            rotationUIModel.RotationListBox = listBox;

            StackPanel stackPanel2 = new StackPanel();
            stackPanel2.Orientation = Orientation.Vertical;
            stackPanel2.Margin = new Thickness(5);
            TextBlock currentlyUpTextBlock = new TextBlock();
            currentlyUpTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            currentlyUpTextBlock.Margin = new Thickness(0, 4, 0, 4);
            currentlyUpTextBlock.Text = $"Currently Up: {rotation.CurrentEmployee}";
            rotationUIModel.CurrentEmployeeTextBlock = currentlyUpTextBlock;
            TextBlock rotationAdvancesTextBlock = new ();
            rotationAdvancesTextBlock.Margin = new Thickness(0, 4, 0, 0);
            rotationAdvancesTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            rotationAdvancesTextBlock.Text = $"Rotation Advances:";
            TextBlock dateTimeTextBlock = new ();
            dateTimeTextBlock.Margin = new Thickness(0, 0, 0, 4);
            dateTimeTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            dateTimeTextBlock.Text = $"{rotation.BasicInfo.NextDateTimeRotationAdvances:g}";
            stackPanel2.Children.Add(currentlyUpTextBlock);
            stackPanel2.Children.Add(rotationAdvancesTextBlock);
            stackPanel2.Children.Add(dateTimeTextBlock);

            StackPanel stackPanel3 = new StackPanel();
            stackPanel3.Orientation = Orientation.Horizontal;
            stackPanel3.HorizontalAlignment = HorizontalAlignment.Center;
            Button advanceButton = new Button();
            advanceButton.DataContext = rotationUIModel;
            advanceButton.Margin = new Thickness(0, 5, 0, 5);
            advanceButton.Width = 80;
            advanceButton.Content = "Advance";
            ShowButtonIfUserIsAdmin(advanceButton);
            advanceButton.Click += AdvanceRotationButton_Click;
            rotationUIModel.AdvanceButton = advanceButton;
            Button editButton = new Button();
            editButton.DataContext = rotationUIModel;
            editButton.Margin = new Thickness(5);
            editButton.Width = 45;
            editButton.Content = "Edit";
            ShowButtonIfUserIsAdmin(editButton);
            editButton.Click += EditRotationButton_Click;
            rotationUIModel.EditButton = editButton;
            stackPanel3.Children.Add(advanceButton);
            stackPanel3.Children.Add(editButton);

            Label notesLabel = new Label();
            notesLabel.Content = "Notes:";
            TextBox notesTextBox = new TextBox();
            rotationUIModel.RotationNotesTextBox = notesTextBox;
            notesTextBox.IsReadOnly = true;
            notesTextBox.TextWrapping = TextWrapping.Wrap;
            notesTextBox.Height = 60;
            notesTextBox.MaxWidth = 215;
            notesTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            notesTextBox.Text = rotation.BasicInfo.Notes;

            stackPanel.Children.Add(listBox);
            stackPanel.Children.Add(stackPanel2);
            stackPanel.Children.Add(stackPanel3);
            stackPanel.Children.Add(notesLabel);
            stackPanel.Children.Add(notesTextBox);

            groupBox.Content = stackPanel;

            rotationUIModels.Add(rotationUIModel);
            rotationsWrapPanel.Children.Add(groupBox);
        }

        private void ShowButtonIfUserIsAdmin(Button button)
        {
            if (currentUserIsAdmin == false)
            {
                button.Visibility = Visibility.Collapsed;
            }
            else if (currentUserIsAdmin == true)
            {
                button.Visibility = Visibility.Visible;
            }
        }

        private void GetCurrentUser()
        {
            currentUser = Environment.UserName;
        }

        private void DisplayCurrentUser()
        {
            userNameTextBlock.Text = currentUser;
        }

        private void ReadAdminsFromDB()
        {
            admins = _db.ReadAllAdmins();
        }

        private void CheckIfCurrentUserIsAdmin()
        {
            foreach (string user in admins)
            {
                if (currentUser == user)
                {
                    currentUserIsAdmin = true;
                }
            }
        }

        private void ShowControlsIfCurrentUserIsAdmin()
        {
            if (currentUserIsAdmin == true)
            {
                editEmployeesButton.Visibility = Visibility.Visible;
                addRotationButton.Visibility = Visibility.Visible;
                removeRotationButton.Visibility = Visibility.Visible;

                foreach (var uiModel in rotationUIModels)
                {
                    uiModel.AdvanceButton.Visibility = Visibility.Visible;
                    uiModel.EditButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void CreateTimer()
        {
            System.Timers.Timer refreshAppTimer = new();
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

        private void AdvanceRotationsIfDateTimeHasPassed()
        {
            foreach (var model in rotationUIModels)
            {
                AdvanceRotationIfDateTimeHasPassed(model);
            }
        }

        private void AdvanceRotationIfDateTimeHasPassed(RotationUIModel rotationUIModel)
        {
            DateTime nextDateTimeRotationAdvances = rotationUIModel.FullRotationModel.BasicInfo.NextDateTimeRotationAdvances;

            if (nextDateTimeRotationAdvances != DateTime.MinValue)
            {
                DateTime now = DateTime.Now;
                bool keepAdvancing = false;

                do
                {
                    nextDateTimeRotationAdvances = rotationUIModel.FullRotationModel.BasicInfo.NextDateTimeRotationAdvances;

                    if (now > nextDateTimeRotationAdvances)
                    {
                        rotationUIModel.FullRotationModel.AdvanceRotation();
                        AdvanceRotationInDB(rotationUIModel.FullRotationModel);
                        rotationUIModel.FullRotationModel.SetNextDateTimeRotationAdvances();

                        if (rotationUIModel.FullRotationModel.RotationOfEmployees.Count > 0)
                        {
                            notificationMessage += $"{rotationUIModel.FullRotationModel.RotationOfEmployees.Last().FullName} " +
                                $"took their turn for {rotationUIModel.FullRotationModel.BasicInfo.RotationName}." +
                                $"{Environment.NewLine}";
                        }

                        rotationUIModel.CurrentEmployeeTextBlock.Text = $"Currently Up: {rotationUIModel.FullRotationModel.CurrentEmployee}";
                        rotationUIModel.RotationListBox.RefreshContents(rotationUIModel.FullRotationModel.RotationOfEmployees);
                        UpdateRotationBasicInfoInDB(rotationUIModel.FullRotationModel.BasicInfo);

                        nextDateTimeRotationAdvances = rotationUIModel.FullRotationModel.BasicInfo.NextDateTimeRotationAdvances;

                        if (now > nextDateTimeRotationAdvances)
                        {
                            keepAdvancing = true;
                        }
                        else
                        {
                            keepAdvancing = false;
                        }
                    }
                    else
                    {
                        keepAdvancing = false;
                    }
                } while (keepAdvancing == true);
            }
        }

        private void AdvanceRotation(RotationUIModel rotationUIModel)
        {
            rotationUIModel.FullRotationModel.AdvanceRotation();
            rotationUIModel.CurrentEmployeeTextBlock.Text = $"Currently Up: {rotationUIModel.FullRotationModel.CurrentEmployee}";

            rotationUIModel.RotationListBox.RefreshContents(rotationUIModel.FullRotationModel.RotationOfEmployees);
            AdvanceRotationInDB(rotationUIModel.FullRotationModel);
        }

        private void EditEmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            EditEmployees editEmployees = new(this);
            editEmployees.ShowDialog();
        }

        private void AddRotationButton_Click(object sender, RoutedEventArgs e)
        {
            RotationUIModel rotationUIModel = new ();

            FullRotationModel rotation = new ();
            rotation.BasicInfo.RotationName = $"Rotation {rotations.Count + 1}";
            rotation.BasicInfo.RotationRecurrence = RecurrenceInterval.Weekly;
            rotation.BasicInfo.NextDateTimeRotationAdvances = DateTime.Now.AddDays(7);
            rotation.RotationOfEmployees = employees;

            rotationUIModel.FullRotationModel = rotation;
            rotations.Add(rotation);

            CreateRotationInDB(rotation);
            ReadRotationsFromDB(); // re-read the rotations to fill in the RotationId of the rotation that was just created

            CreateRotationInUI(rotationUIModel, rotation);
        }

        private void RemoveRotationButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveRotation removeRotation = new(this);
            removeRotation.ShowDialog();
        }

        private void AdvanceRotationButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            RotationUIModel rotationUIModel = (RotationUIModel)button.DataContext;
            AdvanceRotation(rotationUIModel);
        }

        private void EditRotationButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            RotationUIModel rotationUIModel = (RotationUIModel)button.DataContext;
            EditRotation editRotation = new(this, rotationUIModel);
            editRotation.ShowDialog();
        }
    }
}
