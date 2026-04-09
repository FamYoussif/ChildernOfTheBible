using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildernOfTheBible.Models
{
    public class Meeting
    {
        [Key]
        public int MeetingId { get; set; }
        public DateTime MeetingDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Navigation property for attendance records (one-to-many relationship)
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; }

        public Meeting()
        {
            AttendanceRecords = new HashSet<AttendanceRecord>();
        }
    }
}
