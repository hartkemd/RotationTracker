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

        //public void SetNextDateTimeRotationAdvances()
        //{
        //    if (RotationRecurrence == RecurrenceInterval.Weekly)
        //    {
        //        NextDateTimeRotationAdvances = NextDateTimeRotationAdvances.AddDays(7);
        //    }
        //    else if (RotationRecurrence == RecurrenceInterval.Monthly)
        //    {
        //        NextDateTimeRotationAdvances = NextDateTimeRotationAdvances.AddMonths(1);
        //    }
        //    else if (RotationRecurrence == RecurrenceInterval.Bimonthly)
        //    {
        //        NextDateTimeRotationAdvances = NextDateTimeRotationAdvances.AddMonths(2);
        //    }
        //}
    }
}
