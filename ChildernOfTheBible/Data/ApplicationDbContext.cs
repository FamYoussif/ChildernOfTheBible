using ChildernOfTheBible.Models;
using Microsoft.EntityFrameworkCore;


namespace ChildernOfTheBible.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships and constraints that are not covered by conventions or data annotations

            // Ensure BarcodeId in Member is unique
            modelBuilder.Entity<Member>()
                .HasIndex(m => m.BarcodeId)
                .IsUnique();

            // Define the one-to-many relationship between Member and AttendanceRecord
            modelBuilder.Entity<Member>()
                .HasMany(m => m.AttendanceRecords)
                .WithOne(ar => ar.Member)
                .HasForeignKey(ar => ar.MemberId);

            // Define the one-to-many relationship between Meeting and AttendanceRecord
            modelBuilder.Entity<Meeting>()
                .HasMany(m => m.AttendanceRecords)
                .WithOne(ar => ar.Meeting)
                .HasForeignKey(ar => ar.MeetingId);
        }
    }
}
