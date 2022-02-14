
namespace DataAccessLibrary.Models
{
    public class CalendarEventModel
    {
        public int Id { get; set; }
        public int RotationId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool OnCalendar { get; set; }
    }
}
