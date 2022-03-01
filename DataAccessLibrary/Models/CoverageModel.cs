using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class CoverageModel
    {
        public int Id { get; set; }
        public int RotationId { get; set; }
        public int EmployeeIdOfCovering { get; set; }
        public int EmployeeIdOfCovered { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
