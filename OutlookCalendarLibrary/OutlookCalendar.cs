using DataAccessLibrary.Models;
using System.Diagnostics;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OutlookCalendarLibrary
{
    public class OutlookCalendar
    {
        private static Outlook.Application app = new();
        private static Outlook.MAPIFolder calendarFolder = null;
        private static Outlook.Items items = null;
        private static Outlook.AppointmentItem appItem = null;
        private static Outlook.Stores stores = app.Session.Stores;

        public OutlookCalendar(string outlookStoreName)
        {
            PopulateCalendarFolder(outlookStoreName);
        }

        private static void PopulateCalendarFolder(string outlookStoreName)
        {
            foreach (Outlook.Store store in stores)
            {
                if (store.DisplayName == outlookStoreName)
                {
                    calendarFolder = store.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar);
                }
            }
        }

        public static bool OutlookIsRunning()
        {
            bool isRunning;

            if (Process.GetProcessesByName("OUTLOOK").Any())
            {
                isRunning = true;
            }
            else
            {
                isRunning = false;
            }

            return isRunning;
        }

        public bool CalendarFolderIsAccessible()
        {
            bool isAccessible;

            if (calendarFolder == null)
            {
                isAccessible = false;
            }
            else
            {
                isAccessible = true;
            }

            return isAccessible;
        }

        public bool CreateAppointmentItem(BasicRotationModel basicRotation, EmployeeModel employee)
        {
            bool appointmentItemDisplayed = false;

            if (calendarFolder != null)
            {
                items = calendarFolder.Items;
                appItem = items.Add(Outlook.OlItemType.olAppointmentItem) as Outlook.AppointmentItem;
                appItem.Subject = $"{basicRotation.RotationName} - {employee.FullName}";
                appItem.Start = employee.NextStartDateTime;
                appItem.End = employee.NextEndDateTime;
                //appItem.Categories = "Moderator";
                appItem.ReminderSet = false;
                appItem.Display(false);
                appointmentItemDisplayed = true;
            }

            return appointmentItemDisplayed;
        }
    }
}
