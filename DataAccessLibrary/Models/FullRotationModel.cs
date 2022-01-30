using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class FullRotationModel
    {
        public BasicRotationModel BasicInfo { get; set; }
        public List<string> RotationOfEmployees { get; set; } = new List<string>();
    }
}
