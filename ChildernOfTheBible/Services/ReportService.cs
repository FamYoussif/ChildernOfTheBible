// Services/ReportService.cs
using System.Globalization;
using System.IO;
using ChildernOfTheBible.Data;
using ChildernOfTheBible.Models;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ChildernOfTheBible.Services
{
    public record AttendanceRow(string FullName, string BarcodeId,
                                DateTime MeetingDate, DateTime CheckInTime);

    public class ReportService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _factory;
        private readonly EncryptionService _enc;

        public ReportService(IDbContextFactory<ApplicationDbContext> factory, EncryptionService enc)
        {
            _factory = factory;
            _enc = enc;
        }

        public async Task<List<AttendanceRow>> GetMeetingReportAsync(int meetingId)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.AttendanceRecords
                .Where(a => a.MeetingId == meetingId)
                .Include(a => a.Member)
                .Include(a => a.Meeting)
                .Select(a => new AttendanceRow(
                    a.Member.FirstName + " " + a.Member.LastName,
                    a.Member.BarcodeId,
                    a.Meeting.MeetingDate,
                    a.CheckInTime))
                .ToListAsync();
        }

        public async Task<List<AttendanceRow>> GetRangeReportAsync(DateTime from, DateTime to)
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.AttendanceRecords
                .Where(a => a.Meeting.MeetingDate >= from && a.Meeting.MeetingDate <= to)
                .Include(a => a.Member)
                .Include(a => a.Meeting)
                .Select(a => new AttendanceRow(
                    a.Member.FirstName + " " + a.Member.LastName,
                    a.Member.BarcodeId,
                    a.Meeting.MeetingDate,
                    a.CheckInTime))
                .ToListAsync();
        }

        public async Task<List<Meeting>> GetAllMeetingsAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();
            return await context.Meetings
                .OrderByDescending(m => m.MeetingDate)
                .ToListAsync();
        }

        public void ExportToCsv(IEnumerable<AttendanceRow> rows, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture);
            csv.WriteRecords(rows);
        }

        public void ExportToPdf(IEnumerable<AttendanceRow> rows, string filePath, string title)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var rowList = rows.ToList();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Header().Text(title).FontSize(18).Bold();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(3);
                            c.RelativeColumn(2);
                            c.RelativeColumn(2);
                            c.RelativeColumn(2);
                        });
                        table.Header(h =>
                        {
                            h.Cell().Text("Name").Bold();
                            h.Cell().Text("Barcode").Bold();
                            h.Cell().Text("Date").Bold();
                            h.Cell().Text("Check-in").Bold();
                        });
                        foreach (var r in rowList)
                        {
                            table.Cell().Text(r.FullName);
                            table.Cell().Text(r.BarcodeId);
                            table.Cell().Text(r.MeetingDate.ToString("dd/MM/yyyy"));
                            table.Cell().Text(r.CheckInTime.ToString("HH:mm"));
                        }
                    });
                    page.Footer().AlignCenter()
                        .Text($"Generated {DateTime.Now:dd/MM/yyyy HH:mm} — Bible Study Attendance");
                });
            }).GeneratePdf(filePath);
        }
    }
}