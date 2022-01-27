using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationLibrary.Models
{
    public class EmployeeListModel : BaseFileClass
    {
        public List<string> EmployeeList { get; set; } = new List<string>();
    }
}
