using System.Windows;
using System.Windows.Controls;
using ChildernOfTheBible.ViewModels;

namespace ChildernOfTheBible.Views
{
    public partial class UserManagement : UserControl
    {
        public UserManagement()
        {
            InitializeComponent();

            // Auto-load members when view appears
            Loaded += async (s, e) =>
            {
                if (DataContext is UserManagementViewModel vm)
                    await vm.LoadMembersCommand.ExecuteAsync(null);
            };
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not UserManagementViewModel vm) return;

            if (vm.SelectedMember != null)
            {
                // A row is selected — populate the form
                vm.SelectMemberCommand.Execute(vm.SelectedMember);
            }
            else
            {
                // Selection was cleared — clear the form automatically
                vm.ClearSelectionCommand.Execute(null);
            }
        }

        private void PrintBarcode_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new PrintDialog();
            if (dlg.ShowDialog() == true)
                dlg.PrintVisual(BarcodeImg, "Member Barcode");
        }
    }
}