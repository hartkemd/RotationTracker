using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class RotationEmployeeModel
    {
        public int Id { get; set; }
        public int RotationId { get; set; }
        public int EmployeeId { get; set; }
    }
}
