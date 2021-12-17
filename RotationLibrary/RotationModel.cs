using System;
using System.Collections.Generic;

namespace RotationLibrary
{
    public class RotationModel
    {
        public string RotationName { get; set; }

        public List<string> Rotation { get; set; } = new List<string>();

        public string FilePath { get; set; }

        /// <summary>
        /// Represents the time interval after which the rotation advances.
        /// </summary>
        public RecurrenceInterval RotationRecurrence { get; set; }

        public DayOfWeek RotationRecurrenceDayOfWeek { get; set; }

        public DateTime DateTimeRotationAdvances { get; set; }

        /// <summary>
        /// Represents the DateTime when the rotation begins and the employee starts their turn.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Represents the DateTime when the rotation ends and advances to the next employee.
        /// </summary>
        public DateTime EndDate { get; set; }

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
    }
}
