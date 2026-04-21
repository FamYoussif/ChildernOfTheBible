// ViewModels/ReportingViewModel.cs
using System.Collections.ObjectModel;
using ChildernOfTheBible.Models;
using ChildernOfTheBible.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

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

        // ✅ Called automatically when the view loads (wired in code-behind)
        [RelayCommand]
        public async Task LoadMeetingsAsync()
        {
            var list = await _svc.GetAllMeetingsAsync();
            Meetings = new ObservableCollection<Meeting>(list);
            StatusMessage = $"{list.Count} meeting(s) available.";
        }

        [RelayCommand]
        private async Task RunMeetingReportAsync()
        {
            if (SelectedMeeting == null)
            {
                StatusMessage = "Please select a meeting from the dropdown first.";
                return;
            }
            _currentRows = await _svc.GetMeetingReportAsync(SelectedMeeting.MeetingId);
            ReportRows = new ObservableCollection<AttendanceRow>(_currentRows);
            StatusMessage = $"{_currentRows.Count} attendee(s) for " +
                            $"{SelectedMeeting.MeetingDate:dd/MM/yyyy}.";
        }

        [RelayCommand]
        private async Task RunRangeReportAsync()
        {
            if (RangeFrom > RangeTo)
            {
                StatusMessage = "From date cannot be after To date.";
                return;
            }
            _currentRows = await _svc.GetRangeReportAsync(RangeFrom, RangeTo);
            ReportRows = new ObservableCollection<AttendanceRow>(_currentRows);
            StatusMessage = $"{_currentRows.Count} record(s) from " +
                            $"{RangeFrom:dd/MM/yyyy} to {RangeTo:dd/MM/yyyy}.";
        }

        [RelayCommand]
        private void ExportCsv()
        {
            if (_currentRows.Count == 0)
            {
                StatusMessage = "Run a report first before exporting.";
                return;
            }
            var dlg = new SaveFileDialog
            {
                Filter = "CSV files|*.csv",
                FileName = $"attendance_{DateTime.Today:yyyyMMdd}.csv"
            };
            if (dlg.ShowDialog() == true)
            {
                _svc.ExportToCsv(_currentRows, dlg.FileName);
                StatusMessage = $"CSV saved: {dlg.FileName}";
            }
        }

        [RelayCommand]
        private void ExportPdf()
        {
            if (_currentRows.Count == 0)
            {
                StatusMessage = "Run a report first before exporting.";
                return;
            }
            var dlg = new SaveFileDialog
            {
                Filter = "PDF files|*.pdf",
                FileName = $"attendance_{DateTime.Today:yyyyMMdd}.pdf"
            };
            if (dlg.ShowDialog() == true)
            {
                _svc.ExportToPdf(_currentRows, dlg.FileName, Title);
                StatusMessage = $"PDF saved: {dlg.FileName}";
            }
        }
    }
}