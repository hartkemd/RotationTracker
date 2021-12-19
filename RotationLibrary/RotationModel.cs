using System;
using System.Collections.Generic;

namespace RotationLibrary
{
    public class RotationModel : BaseFileClass
    {
        public string RotationName { get; set; }

        public List<string> Rotation { get; set; } = new List<string>();

        public RecurrenceInterval RotationRecurrence { get; set; } = RecurrenceInterval.Weekly;

        public DateTime NextDateTimeRotationAdvances { get; set; }

        public string CurrentEmployee => GetCurrentEmployee();

        private string GetCurrentEmployee()
        {
            if (Rotation.Count > 0)
            {
                return Rotation[0];
            }
            else
            {
                return null;
            }
        }

        public void AdvanceRotation()
        {
            if (Rotation.Count > 0)
            {
                string employeeWhoWent = Rotation[0];
                Rotation.RemoveAt(0);
                Rotation.Add(employeeWhoWent);
            }
        }

        public void SetNextDateTimeRotationAdvances()
        {
            if (RotationRecurrence == RecurrenceInterval.Weekly)
            {
                NextDateTimeRotationAdvances = NextDateTimeRotationAdvances.AddDays(7);
            }
            else if (RotationRecurrence == RecurrenceInterval.Monthly)
            {
                NextDateTimeRotationAdvances = NextDateTimeRotationAdvances.AddMonths(1);
            }
            else if (RotationRecurrence == RecurrenceInterval.Bimonthly)
            {
                NextDateTimeRotationAdvances = NextDateTimeRotationAdvances.AddMonths(2);
            }
        }

        public void Clear()
        {
            RotationName = "";
            Rotation.Clear();
            RotationRecurrence = RecurrenceInterval.Weekly;
            NextDateTimeRotationAdvances = DateTime.MinValue;
        }
    }
}
