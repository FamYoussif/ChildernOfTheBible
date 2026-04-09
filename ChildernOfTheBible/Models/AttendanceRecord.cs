using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChildernOfTheBible.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int AttendanceRecordId { get; set; }

        // Foreign Key to Member
        public int MemberId { get; set; }
        [ForeignKey("MemberId")]
        public Member Member { get; set; }

        // Foreign Key to Meeting
        public int MeetingId { get; set; }
        [ForeignKey("MeetingId")]
        public Meeting Meeting { get; set; }

        public DateTime CheckInTime { get; set; }
    }
}
