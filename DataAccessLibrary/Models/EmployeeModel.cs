
namespace DataAccessLibrary.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime NextStartDateTime { get; set; }
        public DateTime NextEndDateTime { get; set; }
        public bool OnCalendar { get; set; }
    }
}
