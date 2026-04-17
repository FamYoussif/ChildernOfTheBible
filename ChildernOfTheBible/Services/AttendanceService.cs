// Services/AttendanceService.cs
using ChildernOfTheBible.Data;
using ChildernOfTheBible.Models;
using Microsoft.EntityFrameworkCore;

namespace ChildernOfTheBible.Services
{
    public class AttendanceService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;

        public AttendanceService(IDbContextFactory<ApplicationDbContext> factory)
            => _factory = factory;

        public async Task<Meeting> StartMeetingAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            var meeting = new Meeting
            {
                MeetingDate = DateTime.Today,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now
            };
            context.Meetings.Add(meeting);
            await context.SaveChangesAsync();
            return meeting;
        }

        public async Task EndMeetingAsync(int meetingId)
        {
            await using var context = await _factory.CreateDbContextAsync();
            var m = await context.Meetings.FindAsync(meetingId);
            if (m != null)
            {
                m.EndTime = DateTime.Now;
                await context.SaveChangesAsync();
            }
        }

        public async Task<Member?> RecordAttendanceAsync(string barcodeId, int meetingId)
        {
            await using var context = await _factory.CreateDbContextAsync();

            var member = await context.Members
                .FirstOrDefaultAsync(m => m.BarcodeId == barcodeId && m.IsActive);
            if (member == null) return null;

            var exists = await context.AttendanceRecords
                .AnyAsync(a => a.MemberId == member.MemberId && a.MeetingId == meetingId);
            if (exists) return null;

            context.AttendanceRecords.Add(new AttendanceRecord
            {
                MemberId = member.MemberId,
                MeetingId = meetingId,
                CheckInTime = DateTime.Now
            });
            await context.SaveChangesAsync();
            return member;
        }
    }
}