using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChildernOfTheBible.ViewModels
{
    public partial class AttendanceViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Attendance Check-in";
    }
}
