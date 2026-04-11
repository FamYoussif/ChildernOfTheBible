using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ChildernOfTheBible.ViewModels;

namespace ChildernOfTheBible.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _currentViewModel;

        public MainViewModel(AttendanceViewModel attendanceViewModel, UserManagementViewModel memberViewModel, ReportingViewModel reportViewModel)
        {
            _attendanceViewModel = attendanceViewModel;
            _memberViewModel = memberViewModel;
            _reportViewModel = reportViewModel;

            // Set the default view
            CurrentViewModel = _attendanceViewModel;
        }

        private readonly AttendanceViewModel _attendanceViewModel;
        private readonly UserManagementViewModel _memberViewModel;
        private readonly ReportingViewModel _reportViewModel;

        [RelayCommand]
        private void NavigateToAttendance() => CurrentViewModel = _attendanceViewModel;

        [RelayCommand]
        private void NavigateToMembers() => CurrentViewModel = _memberViewModel;

        [RelayCommand]
        private void NavigateToReports() => CurrentViewModel = _reportViewModel;
    }
}
