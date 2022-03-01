using RotationLibrary;

namespace DataAccessLibrary.Models
{
    public class BasicRotationModel
    {
        public int Id { get; set; }
        public string RotationName { get; set; }
        public RecurrenceInterval RotationRecurrence { get; set; }
        public DateTime NextDateTimeRotationAdvances { get; set; }
        public bool AdvanceAutomatically { get; set; }
        public string Notes { get; set; }
        public string OutlookCategory { get; set; }
    }
}
