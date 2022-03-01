using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using Microsoft.Extensions.Configuration;
using RotationLibrary;
using RotationTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WPFHelperLibrary;

namespace RotationTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ISqliteData _db;
        private string _outlookStoreName;
        public List<EmployeeModel> employees = new ();
        public List<FullRotationModel> rotations = new ();
        public List<RotationUIModel> rotationUIModels = new ();
        public List<CoverageReadModel> coverages = new ();
        private List<string> admins = new();
        private string currentUser;
        private bool currentUserIsAdmin = false;
        private string notificationMessage;

        public MainWindow(ISqliteData db, IConfiguration config)
        {
            InitializeComponent();
            _db = db;
            _outlookStoreName = config.GetValue<string>("OutlookStoreName");

            ReadEmployeesFromDBAsync();
            employeeListBox.ItemsSource = employees;
            ReadRotationsFromDBAsync();
            LoadRotationsIntoUI();
            ReadCoveragesFromDBAsync();
            SetCoveragesInactiveIfOlderThanOneYear();

            GetCurrentUser();
            DisplayCurrentUser();
            ReadAdminsFromDBAsync();
            CheckIfCurrentUserIsAdmin();
            ShowControlsIfCurrentUserIsAdmin();

            AdvanceRotationsIfDateTimeHasPassed();

            DisplayNotificationsAsync();
            CreateTimer();
        }

        private void SetCoveragesInactiveIfOlderThanOneYear()
        {
            foreach (var coverage in coverages)
            {
                DateTime endDate = coverage.EndDate.AddYears(1);
                if (endDate.Date < DateTime.Now.Date)
                {
                    SetCoverageInactiveInDBAsync(coverage.Id);
                }
            }
        }

        public async void ReadCoveragesFromDBAsync()
        {
            coverages = await _db.ReadAllCoveragesAsync();
        }

        public async Task<ObservableCollection<CoverageReadModel>> ReadCoveragesForRotationAsync(int rotationId)
        {
            ObservableCollection<CoverageReadModel> output = new (await _db.ReadCoveragesForRotationAsync(rotationId));
            return output;
        }

        public async void CreateCoverageInDBAsync(CoverageModel coverage)
        {
            await _db.CreateCoverageAsync(coverage);
        }

        public async void SetCoverageInactiveInDBAsync(int coverageId)
        {
            await _db.SetCoverageInactiveAsync(coverageId);
        }

        public async void DeleteCoverageFromDBAsync(int coverageId)
        {
            await _db.DeleteCoverageAsync(coverageId);
        }

        public async void ReadEmployeesFromDBAsync()
        {
            employees = await _db.GetAllEmployeesAsync();
        }

        public async void ReadRotationsFromDBAsync()
        {
            rotations = await _db.GetAllRotationsAsync();
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

        public async void CreateEmployeeInDBAsync(string employeeName)
        {
            await _db.CreateEmployeeAsync(employeeName);
        }

        public async void DeleteEmployeeFromDBAsync(int id)
        {
            await _db.DeleteEmployeeAsync(id);
        }

        private async void CreateRotationInDBAsync(FullRotationModel fullRotation)
        {
            await _db.CreateRotationAsync(fullRotation);
        }

        public async void RecreateRotationInDBAsync(FullRotationModel fullRotation)
        {
            await _db.RecreateRotationOfEmployeesAsync(fullRotation);
        }

        private async Task<int> GetHighestIdFromRotations()
        {
            return await _db.GetHighestIdFromRotationsAsync();
        }

        public async void UpdateRotationBasicInfoInDBAsync(BasicRotationModel basicRotation)
        {
            await _db.UpdateRotationBasicInfoAsync(basicRotation);
        }

        private async void AdvanceRotationInDBAsync(FullRotationModel fullRotation)
        {
            await _db.AdvanceRotationAsync(fullRotation);
        }

        private async void ReverseRotationInDBAsync(FullRotationModel fullRotation)
        {
            await _db.ReverseRotationAsync(fullRotation);
        }

        public async void DeleteRotationFromDBAsync(int id)
        {
            await _db.DeleteRotationAsync(id);
        }

        public async void UpdateOnCalendarInDBAsync(BasicRotationModel basicRotation, EmployeeModel employee, bool onCalendar)
        {
            await _db.UpdateOnCalendarAsync(basicRotation, employee, onCalendar);
        }

        private void CreateRotationInUI(RotationUIModel rotationUIModel, FullRotationModel rotation)
        {
            GroupBox groupBox = new ();
            groupBox.Margin = new Thickness(5);
            Label label = new ();
            label.Content = $"{rotation.BasicInfo.RotationName}:";
            rotationUIModel.RotationNameLabel = label;
            groupBox.Header = label;

            StackPanel stackPanel = new ();
            stackPanel.Orientation = Orientation.Vertical;
            ListBox listBox = new ();
            listBox.Margin = new Thickness(5, 0, 5, 0);
            listBox.ItemsSource = rotation.RotationOfEmployees;

            PopulateRotationListBox(rotation, listBox);

            rotationUIModel.RotationListBox = listBox;

            StackPanel stackPanel2 = new ();
            stackPanel2.Orientation = Orientation.Vertical;
            stackPanel2.Margin = new Thickness(5);
            TextBlock currentlyUpTextBlock = new ();
            currentlyUpTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            currentlyUpTextBlock.Margin = new Thickness(0, 4, 0, 4);
            currentlyUpTextBlock.Text = $"Currently Up: {rotation.CurrentEmployeeName}";
            rotationUIModel.CurrentEmployeeTextBlock = currentlyUpTextBlock;
            TextBlock rotationAdvancesTextBlock = new ();
            rotationAdvancesTextBlock.Margin = new Thickness(0, 4, 0, 0);
            rotationAdvancesTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            rotationAdvancesTextBlock.Text = "Rotation Advances:";
            TextBlock dateTimeTextBlock = new ();
            dateTimeTextBlock.Margin = new Thickness(0, 0, 0, 4);
            dateTimeTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            dateTimeTextBlock.Text = $"{rotation.BasicInfo.NextDateTimeRotationAdvances:g}";
            rotationUIModel.DateTimeTextBlock = dateTimeTextBlock;
            stackPanel2.Children.Add(currentlyUpTextBlock);
            stackPanel2.Children.Add(rotationAdvancesTextBlock);
            stackPanel2.Children.Add(dateTimeTextBlock);

            StackPanel stackPanel3 = new ();
            stackPanel3.Orientation = Orientation.Horizontal;
            stackPanel3.HorizontalAlignment = HorizontalAlignment.Center;
            Button advanceButton = new ();
            advanceButton.DataContext = rotationUIModel;
            advanceButton.Margin = new Thickness(5);
            advanceButton.Width = 80;
            advanceButton.Content = "Advance";
            ShowButtonIfUserIsAdmin(advanceButton);
            advanceButton.Click += AdvanceRotationButton_Click;
            rotationUIModel.AdvanceButton = advanceButton;
            Button reverseButton = new ();
            reverseButton.DataContext = rotationUIModel;
            reverseButton.Margin = new Thickness(0, 5, 0, 5);
            reverseButton.Width = 80;
            reverseButton.Content = "Reverse";
            ShowButtonIfUserIsAdmin(reverseButton);
            reverseButton.Click += ReverseRotationButton_Click;
            rotationUIModel.ReverseButton = reverseButton;
            Button editButton = new ();
            editButton.DataContext = rotationUIModel;
            editButton.Margin = new Thickness(5);
            editButton.Width = 45;
            editButton.Content = "Edit";
            ShowButtonIfUserIsAdmin(editButton);
            editButton.Click += EditRotationButton_Click;
            rotationUIModel.EditButton = editButton;
            stackPanel3.Children.Add(advanceButton);
            stackPanel3.Children.Add(reverseButton);
            stackPanel3.Children.Add(editButton);

            Label notesLabel = new ();
            notesLabel.Content = "Notes:";
            TextBox notesTextBox = new ();
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

        public void PopulateRotationListBox(FullRotationModel rotation, ListBox listBox)
        {
            string dataTemplateString;

            rotation.PopulateNextStartDateTimesOfEmployees();
            rotation.PopulateNextEndDateTimesOfEmployees();

            if (rotation.BasicInfo.RotationRecurrence == RecurrenceInterval.Weekly ||
                rotation.BasicInfo.RotationRecurrence == RecurrenceInterval.WeeklyWorkWeek)
            {
                dataTemplateString = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                                                                    <StackPanel Orientation=""Horizontal"">
                                                                        <TextBlock Text=""{Binding Path=FullName}"" />
                                                                        <TextBlock Text="" ("" />
                                                                        <TextBlock Text=""{Binding Path=NextStartDateTime, StringFormat=d}"" />
                                                                        <TextBlock Text="" - "" />
                                                                        <TextBlock Text=""{Binding Path=NextEndDateTime, StringFormat=d}"" />
                                                                        <TextBlock Text="")"" />
                                                                    </StackPanel>
                                                                </DataTemplate>";
                
            }
            else if (rotation.BasicInfo.RotationRecurrence == RecurrenceInterval.BiweeklyOnDay)
            {
                dataTemplateString = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                                                                    <StackPanel Orientation=""Horizontal"">
                                                                        <TextBlock Text=""{Binding Path=FullName}"" />
                                                                        <TextBlock Text="" ("" />
                                                                        <TextBlock Text=""{Binding Path=NextStartDateTime, StringFormat=d}"" />
                                                                        <TextBlock Text="")"" />
                                                                    </StackPanel>
                                                                </DataTemplate>";
            }
            else
            {
                dataTemplateString = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                                                                    <StackPanel Orientation=""Horizontal"">
                                                                        <TextBlock Text=""{Binding Path=FullName}"" />
                                                                        <TextBlock Text="" ("" />
                                                                        <TextBlock Text=""{Binding Path=NextStartDateTime, StringFormat=MMMM}"" />
                                                                        <TextBlock Text="")"" />
                                                                    </StackPanel>
                                                                </DataTemplate>";
            }

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(dataTemplateString));
            var dataTemplate = (DataTemplate)XamlReader.Load(ms);
            listBox.ItemTemplate = dataTemplate;
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

        private async void ReadAdminsFromDBAsync()
        {
            admins = await _db.ReadAllAdminsAsync();
        }

        private void CheckIfCurrentUserIsAdmin()
        {
            foreach (string user in admins)
            {
                if (currentUser.ToLower() == user.ToLower())
                {
                    currentUserIsAdmin = true;
                }
            }
        }

        private void ShowControlsIfCurrentUserIsAdmin()
        {
            if (currentUserIsAdmin == true)
            {
                employeesStackPanel.Visibility = Visibility.Visible;

                foreach (var uiModel in rotationUIModels)
                {
                    uiModel.AdvanceButton.Visibility = Visibility.Visible;
                    uiModel.ReverseButton.Visibility = Visibility.Visible;
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
                if (model.FullRotationModel.BasicInfo.AdvanceAutomatically == true)
                {
                    AdvanceRotationIfDateTimeHasPassed(model);
                }
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
                        AdvanceRotationInDBAsync(rotationUIModel.FullRotationModel);
                        rotationUIModel.FullRotationModel.SetNextDateTimeRotationAdvances();

                        if (rotationUIModel.FullRotationModel.RotationOfEmployees.Count > 0)
                        {
                            notificationMessage += $"{rotationUIModel.FullRotationModel.RotationOfEmployees.Last().FullName} " +
                                $"took their turn for {rotationUIModel.FullRotationModel.BasicInfo.RotationName}." +
                                $"{Environment.NewLine}";
                        }

                        rotationUIModel.CurrentEmployeeTextBlock.Text = $"Currently Up: {rotationUIModel.FullRotationModel.CurrentEmployeeName}";
                        //rotationUIModel.RotationListBox.RefreshContents(rotationUIModel.FullRotationModel.RotationOfEmployees);
                        UpdateRotationBasicInfoInDBAsync(rotationUIModel.FullRotationModel.BasicInfo);

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
            rotationUIModel.CurrentEmployeeTextBlock.Text = $"Currently Up: {rotationUIModel.FullRotationModel.CurrentEmployeeName}";

            PopulateRotationListBox(rotationUIModel.FullRotationModel, rotationUIModel.RotationListBox);
            AdvanceRotationInDBAsync(rotationUIModel.FullRotationModel);
        }

        private void ReverseRotation(RotationUIModel rotationUIModel)
        {
            rotationUIModel.FullRotationModel.ReverseRotation();
            rotationUIModel.CurrentEmployeeTextBlock.Text = $"Currently Up: {rotationUIModel.FullRotationModel.CurrentEmployeeName}";

            PopulateRotationListBox(rotationUIModel.FullRotationModel, rotationUIModel.RotationListBox);
            ReverseRotationInDBAsync(rotationUIModel.FullRotationModel);
        }

        private void EditEmployeesButton_Click(object sender, RoutedEventArgs e)
        {
            EditEmployees editEmployees = new(this);
            editEmployees.ShowDialog();
        }

        private async void AddRotationButton_Click(object sender, RoutedEventArgs e)
        {
            RotationUIModel rotationUIModel = new ();

            FullRotationModel fullRotation = new ();
            fullRotation.BasicInfo.RotationName = $"Rotation {rotations.Count + 1}";
            fullRotation.BasicInfo.RotationRecurrence = RecurrenceInterval.Weekly;
            fullRotation.BasicInfo.NextDateTimeRotationAdvances = DateTime.Now.AddDays(7);
            fullRotation.BasicInfo.AdvanceAutomatically = true;
            fullRotation.RotationOfEmployees = new ObservableCollection<EmployeeModel>(employees);

            rotationUIModel.FullRotationModel = fullRotation;
            rotations.Add(fullRotation);

            CreateRotationInDBAsync(fullRotation);
            fullRotation.BasicInfo.Id = await GetHighestIdFromRotations(); // set the id of the rotation; we need it next
            CreateRotationInUI(rotationUIModel, fullRotation);
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

        private void ReverseRotationButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            RotationUIModel rotationUIModel = (RotationUIModel)button.DataContext;
            ReverseRotation(rotationUIModel);
        }

        private void EditRotationButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            RotationUIModel rotationUIModel = (RotationUIModel)button.DataContext;
            EditRotation editRotation = new(this, rotationUIModel, _outlookStoreName);
            editRotation.ShowDialog();
        }

        private void ShowCoveragesButton_Click(object sender, RoutedEventArgs e)
        {
            ReadCoveragesFromDBAsync();
            Coverages coverages = new(this);
            coverages.ShowDialog();
        }
    }
}
