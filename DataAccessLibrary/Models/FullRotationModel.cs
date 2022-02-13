using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using RotationLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class FullRotationModel
    {
        public BasicRotationModel BasicInfo { get; set; } = new BasicRotationModel();
        public List<EmployeeModel> RotationOfEmployees { get; set; } = new List<EmployeeModel>();
        public string CurrentEmployeeName => GetCurrentEmployeesName();

        public FullRotationModel()
        {
            PopulateNextStartDateTimesOfEmployees();
            PopulateNextEndDateTimesOfEmployees();
        }

        private string GetCurrentEmployeesName()
        {
            if (RotationOfEmployees.Count > 0)
            {
                return RotationOfEmployees[0].FullName;
            }
            else
            {
                return null;
            }
        }

        public void PopulateNextStartDateTimesOfEmployees()
        {
            if (RotationOfEmployees.Count > 0)
            {
                if (BasicInfo.RotationRecurrence == RecurrenceInterval.Weekly)
                {
                    for (int i = 1; i < RotationOfEmployees.Count; i++) // skip the currently up employee
                    {
                        if (i == 1)
                        {
                            RotationOfEmployees[i].NextStartDateTime = BasicInfo.NextDateTimeRotationAdvances;
                        }
                        else
                        {
                            RotationOfEmployees[i].NextStartDateTime = RotationOfEmployees[i - 1].NextStartDateTime.AddDays(7);
                        }
                    }

                    RotationOfEmployees[0].NextStartDateTime = BasicInfo.NextDateTimeRotationAdvances.AddDays(-7);
                }
                else
                {
                    for (int i = 0; i < RotationOfEmployees.Count; i++)
                    {
                        if (i == 0)
                        {
                            RotationOfEmployees[i].NextStartDateTime = BasicInfo.NextDateTimeRotationAdvances;
                        }
                        else
                        {
                            switch (BasicInfo.RotationRecurrence)
                            {
                                case RecurrenceInterval.BiweeklyOnDay:
                                    RotationOfEmployees[i].NextStartDateTime = RotationOfEmployees[i - 1].NextStartDateTime.AddDays(14);
                                    break;
                                case RecurrenceInterval.MonthlyOnDay:
                                    RotationOfEmployees[i].NextStartDateTime = GetNextOccurrence(RotationOfEmployees[i - 1].NextStartDateTime, FrequencyType.Monthly, 1);
                                    break;
                                case RecurrenceInterval.BimonthlyOnDay:
                                    //RotationOfEmployees[i].NextStartDateTime = GetNextOccurrence(FrequencyType.Monthly, 2);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void PopulateNextEndDateTimesOfEmployees()
        {
            if (RotationOfEmployees.Count > 0)
            {
                for (int i = 0; i < RotationOfEmployees.Count; i++)
                {
                    if (i == 0)
                    {
                        RotationOfEmployees[i].NextEndDateTime = BasicInfo.NextDateTimeRotationAdvances.AddMinutes(-1);
                    }
                    else
                    {
                        switch (BasicInfo.RotationRecurrence)
                        {
                            case RecurrenceInterval.Weekly:
                                RotationOfEmployees[i].NextEndDateTime = RotationOfEmployees[i - 1].NextEndDateTime.AddDays(7);
                                break;
                            case RecurrenceInterval.BiweeklyOnDay:
                                RotationOfEmployees[i].NextEndDateTime = RotationOfEmployees[i - 1].NextEndDateTime.AddDays(14);
                                break;
                            case RecurrenceInterval.MonthlyOnDay:
                                RotationOfEmployees[i].NextEndDateTime = RotationOfEmployees[i - 1].NextEndDateTime.AddMonths(1);
                                break;
                            case RecurrenceInterval.BimonthlyOnDay:
                                RotationOfEmployees[i].NextEndDateTime = RotationOfEmployees[i - 1].NextEndDateTime.AddMonths(2);
                                break;
                        }
                    }
                }
            }
        }

        public void AdvanceRotation()
        {
            if (RotationOfEmployees.Count > 0)
            {
                EmployeeModel employeeWhoWent = RotationOfEmployees[0];
                RotationOfEmployees.RemoveAt(0);
                RotationOfEmployees.Add(employeeWhoWent);
            }
        }

        public void ReverseRotation()
        {
            if (RotationOfEmployees.Count > 0)
            {
                int index = RotationOfEmployees.Count - 1;
                EmployeeModel employeeToPutFirst = RotationOfEmployees[index];
                RotationOfEmployees.RemoveAt(index);
                RotationOfEmployees.Insert(0, employeeToPutFirst);
            }
        }

        public DateTime GetNextOccurrence(DateTime searchStartDateTime, FrequencyType frequencyType, int interval)
        {
            DateTime output = DateTime.MinValue;

            var startDate = BasicInfo.NextDateTimeRotationAdvances;
            var endDate = startDate.AddHours(1);

            var rrule = new RecurrencePattern(frequencyType, interval)
            {
                Count = RotationOfEmployees.Count,
                ByDay = new List<WeekDay> { new WeekDay { DayOfWeek = searchStartDateTime.DayOfWeek, Offset = 3 } } // need to find a way to pass in nth day of week in the month
            };

            var e = new CalendarEvent
            {
                Start = new CalDateTime(startDate),
                End = new CalDateTime(endDate),
                RecurrenceRules = new List<RecurrencePattern> { rrule },
            };

            var calendar = new Calendar();
            calendar.Events.Add(e);

            var searchStart = searchStartDateTime.AddDays(1);
            var searchEnd = searchStartDateTime.AddMonths(interval).AddDays(7);
            var occurrences = calendar.GetOccurrences(searchStart, searchEnd);

            foreach (Occurrence occurrence in occurrences)
            {
                output = DateTime.Parse(occurrence.Period.StartTime.ToString());
            }

            return output;
        }

        public void SetNextDateTimeRotationAdvances()
        {
            if (BasicInfo.RotationRecurrence == RecurrenceInterval.Weekly)
            {
                BasicInfo.NextDateTimeRotationAdvances = BasicInfo.NextDateTimeRotationAdvances.AddDays(7);
            }
            else if (BasicInfo.RotationRecurrence == RecurrenceInterval.BiweeklyOnDay)
            {
                BasicInfo.NextDateTimeRotationAdvances = BasicInfo.NextDateTimeRotationAdvances.AddDays(14);
            }
            else if (BasicInfo.RotationRecurrence == RecurrenceInterval.MonthlyOnDay)
            {
                BasicInfo.NextDateTimeRotationAdvances = BasicInfo.NextDateTimeRotationAdvances.AddMonths(1);
            }
            else if (BasicInfo.RotationRecurrence == RecurrenceInterval.BimonthlyOnDay)
            {
                BasicInfo.NextDateTimeRotationAdvances = BasicInfo.NextDateTimeRotationAdvances.AddMonths(2);
            }
        }
    }
}
