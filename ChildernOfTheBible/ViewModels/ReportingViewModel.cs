// ViewModels/ReportingViewModel.cs
using System.Collections.ObjectModel;
using ChildernOfTheBible.Models;
using ChildernOfTheBible.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using QuestPDF;

namespace ChildernOfTheBible.ViewModels
{
    public partial class ReportingViewModel : ObservableObject
    {
        private readonly ReportService _svc;
        private List<AttendanceRow> _currentRows = new();

        [ObservableProperty] private string _title = "Reports & Export";
        [ObservableProperty] private ObservableCollection<Meeting> _meetings = new();
        [ObservableProperty] private Meeting? _selectedMeeting;
        [ObservableProperty] private DateTime _rangeFrom = DateTime.Today.AddDays(-30);
        [ObservableProperty] private DateTime _rangeTo = DateTime.Today;
        [ObservableProperty] private ObservableCollection<AttendanceRow> _reportRows = new();
        [ObservableProperty] private string _statusMessage = "";

        public ReportingViewModel(ReportService svc) => _svc = svc;

        [RelayCommand]
        private async Task LoadMeetingsAsync()
            => Meetings = new ObservableCollection<Meeting>(await _svc.GetAllMeetingsAsync());

        [RelayCommand]
        private async Task RunMeetingReportAsync()
        {
            if (SelectedMeeting == null) return;
            _currentRows = await _svc.GetMeetingReportAsync(SelectedMeeting.MeetingId);
            ReportRows = new ObservableCollection<AttendanceRow>(_currentRows);
            StatusMessage = $"{_currentRows.Count} attendees for {SelectedMeeting.MeetingDate:dd/MM/yyyy}.";
        }

        [RelayCommand]
        private async Task RunRangeReportAsync()
        {
            _currentRows = await _svc.GetRangeReportAsync(RangeFrom, RangeTo);
            ReportRows = new ObservableCollection<AttendanceRow>(_currentRows);
            StatusMessage = $"{_currentRows.Count} records from {RangeFrom:dd/MM/yyyy} to {RangeTo:dd/MM/yyyy}.";
        }

        [RelayCommand]
        private void ExportCsv()
        {
            var dlg = new SaveFileDialog { Filter = "CSV|*.csv", FileName = "attendance.csv" };
            if (dlg.ShowDialog() == true)
            {
                _svc.ExportToCsv(_currentRows, dlg.FileName);
                StatusMessage = $"CSV saved to {dlg.FileName}";
            }
        }

        [RelayCommand]
        private void ExportPdf()
        {
            var dlg = new SaveFileDialog { Filter = "PDF|*.pdf", FileName = "attendance.pdf" };
            if (dlg.ShowDialog() == true)
            {
                _svc.ExportToPdf(_currentRows, dlg.FileName, Title);
                StatusMessage = $"PDF saved to {dlg.FileName}";
            }
        }
    }
}
