using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class CoverageReadModel
    {
        public string RotationName { get; set; }
        public string EmployeeCovering { get; set; }
        public string EmployeeCovered { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CoverageItem => $"{EmployeeCovering} covered for {EmployeeCovered} on {StartDate:d} - {EndDate:d}";
        public string CoverageItemFull => $"{EmployeeCovering} covered for {EmployeeCovered} on {StartDate:d} - {EndDate:d} for {RotationName}";
    }
}
