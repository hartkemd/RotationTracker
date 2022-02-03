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
        public string CurrentEmployee => GetCurrentEmployee();

        private string GetCurrentEmployee()
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

        public void AdvanceRotation()
        {
            if (RotationOfEmployees.Count > 0)
            {
                EmployeeModel employeeWhoWent = RotationOfEmployees[0];
                RotationOfEmployees.RemoveAt(0);
                RotationOfEmployees.Add(employeeWhoWent);
            }
        }

        public void SetNextDateTimeRotationAdvances()
        {
            if (BasicInfo.RotationRecurrence == RecurrenceInterval.Weekly)
            {
                BasicInfo.NextDateTimeRotationAdvances = BasicInfo.NextDateTimeRotationAdvances.AddDays(7);
            }
            else if (BasicInfo.RotationRecurrence == RecurrenceInterval.Monthly)
            {
                BasicInfo.NextDateTimeRotationAdvances = BasicInfo.NextDateTimeRotationAdvances.AddMonths(1);
            }
            else if (BasicInfo.RotationRecurrence == RecurrenceInterval.Bimonthly)
            {
                BasicInfo.NextDateTimeRotationAdvances = BasicInfo.NextDateTimeRotationAdvances.AddMonths(2);
            }
        }
    }
}
