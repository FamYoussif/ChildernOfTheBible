using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ChildernOfTheBible.ViewModels
{
    public partial class UserManagementViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Member Management";
    }
}
