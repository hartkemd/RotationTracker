using RotationLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Models
{
    public class BasicRotationModel
    {
        public int Id { get; set; }
        public string RotationName { get; set; }
        public RecurrenceInterval RotationRecurrence { get; set; }
        public DateTime NextDateTimeRotationAdvances { get; set; }
        public string Notes { get; set; }
    }
}
