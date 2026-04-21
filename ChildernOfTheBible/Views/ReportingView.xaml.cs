// Views/ReportingView.xaml.cs
using System.Windows.Controls;
using ChildernOfTheBible.ViewModels;

namespace ChildernOfTheBible.Views
{
    public partial class ReportingView : UserControl
    {
        public ReportingView()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                if (DataContext is ReportingViewModel vm)
                    await vm.LoadMeetingsCommand.ExecuteAsync(null);
            };
        }
    }
}