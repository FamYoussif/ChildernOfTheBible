using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChildernOfTheBible.ViewModels;

namespace ChildernOfTheBible.Views
{
    /// <summary>
    /// Interaction logic for AttendanceView.xaml
    /// </summary>
    public partial class AttendanceView : UserControl
    {
        public AttendanceView()
        {
            InitializeComponent();
        }
        // Views/AttendanceView.xaml.cs — add inside class
        private void BarcodeBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is AttendanceViewModel vm)
                vm.ProcessBarcodeCommand.Execute(null);
        }
    }
}
