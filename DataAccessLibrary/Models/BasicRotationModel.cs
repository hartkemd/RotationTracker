using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
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

        private void CreateCalendar()
        {
            var now = DateTime.Now;
            var later = now.AddHours(1);

            // repeat monthly for 5 days
            var rrule = new RecurrencePattern(FrequencyType.Monthly, 1) { Count = 5 };

            var e = new CalendarEvent
            {
                Start = new CalDateTime(now),
                End = new CalDateTime(later),
                RecurrenceRules = new List<RecurrencePattern> { rrule },
            };

            var calendar = new Calendar();
            calendar.Events.Add(e);

            //var serializer = new CalendarSerializer();
            //var serializedCalendar = serializer.SerializeToString(calendar);
        }
    }
}
