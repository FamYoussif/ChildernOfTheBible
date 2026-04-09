using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ChildernOfTheBible.Models
{
    public class Member
    {
        [Key] // Denotes MemberId as the primary key
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required] // BarcodeId is mandatory
        public string BarcodeId { get; set; } // Unique identifier for barcode
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true; // Default to active

        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } // Navigation property

        public Member()
        {
            AttendanceRecords = new HashSet<AttendanceRecord>();
        }
    }
}
