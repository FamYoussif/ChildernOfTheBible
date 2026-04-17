// ViewModels/AttendanceViewModel.cs
using System.Collections.ObjectModel;
using ChildernOfTheBible.Models;
using ChildernOfTheBible.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChildernOfTheBible.ViewModels
{
    public partial class AttendanceViewModel : ObservableObject
    {
        private readonly AttendanceService _svc;
        private Meeting? _activeMeeting;

        [ObservableProperty] private string _title = "Attendance Check-in";
        [ObservableProperty] private string _barcodeInput = "";
        [ObservableProperty] private string _statusMessage = "No active meeting. Press Start Meeting.";
        [ObservableProperty] private string _statusColor = "Gray";
        [ObservableProperty] private bool _meetingActive = false;
        [ObservableProperty] private ObservableCollection<string> _recentCheckins = new();

        public AttendanceViewModel(AttendanceService svc) => _svc = svc;

        [RelayCommand]
        private async Task StartMeetingAsync()
        {
            _activeMeeting = await _svc.StartMeetingAsync();
            MeetingActive = true;
            StatusMessage = $"Meeting #{_activeMeeting.MeetingId} started. Ready to scan.";
            StatusColor = "#22C55E";
        }

        [RelayCommand]
        private async Task EndMeetingAsync()
        {
            if (_activeMeeting == null) return;
            await _svc.EndMeetingAsync(_activeMeeting.MeetingId);
            MeetingActive = false;
            StatusMessage = $"Meeting #{_activeMeeting.MeetingId} ended.";
            StatusColor = "Gray";
            _activeMeeting = null;
        }

        // Called when barcode scanner sends Enter key or webcam decodes
        [RelayCommand]
        private async Task ProcessBarcodeAsync()
        {
            if (_activeMeeting == null || string.IsNullOrWhiteSpace(BarcodeInput)) return;
            var member = await _svc.RecordAttendanceAsync(BarcodeInput.Trim(), _activeMeeting.MeetingId);
            if (member != null)
            {
                StatusMessage = $"Welcome, {member.FirstName} {member.LastName}!";
                StatusColor = "#22C55E";
                RecentCheckins.Insert(0, $"{DateTime.Now:HH:mm} — {member.FirstName} {member.LastName}");
            }
            else
            {
                StatusMessage = "Not found or already checked in.";
                StatusColor = "#EF4444";
            }
            BarcodeInput = "";
        }
    }
}
