// ViewModels/UserManagementViewModel.cs
using System.Collections.ObjectModel;
using ChildernOfTheBible.Models;
using ChildernOfTheBible.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ChildernOfTheBible.ViewModels
{
    public partial class UserManagementViewModel : ObservableObject
    {
        private readonly MemberService _svc;
        private readonly BarcodeService _barcode;
        private readonly EncryptionService _enc;

        [ObservableProperty] private string _title = "Member Management";
        [ObservableProperty] private ObservableCollection<Member> _members = new();
        [ObservableProperty] private Member? _selectedMember;
        [ObservableProperty] private string _firstName = "";
        [ObservableProperty] private string _lastName = "";
        [ObservableProperty] private string _email = "";
        [ObservableProperty] private string _phone = "";
        [ObservableProperty] private string _searchText = "";
        [ObservableProperty] private System.Windows.Media.Imaging.BitmapImage? _barcodeImage;
        [ObservableProperty] private string _statusMessage = "";

        public UserManagementViewModel(MemberService svc, BarcodeService barcode, EncryptionService enc)
        {
            _svc = svc; _barcode = barcode; _enc = enc;
        }

        [RelayCommand]
        private async Task LoadMembersAsync()
        {
            var list = await _svc.GetAllAsync();
            Members = new ObservableCollection<Member>(list);
        }

        [RelayCommand]
        private async Task AddMemberAsync()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            { StatusMessage = "First and last name are required."; return; }

            var m = new Member
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                PhoneNumber = Phone,
                IsActive = true
            };
            await _svc.AddAsync(m);
            BarcodeImage = _barcode.GenerateBarcode(m.BarcodeId);
            StatusMessage = $"Member {m.FirstName} added. Barcode: {m.BarcodeId}";
            ClearForm();
            await LoadMembersAsync();
        }

        [RelayCommand]
        private void SelectMember(Member m)
        {
            SelectedMember = m;
            FirstName = m.FirstName;
            LastName = m.LastName;
            Email = _enc.Decrypt(m.Email);
            Phone = _enc.Decrypt(m.PhoneNumber);
            BarcodeImage = _barcode.GenerateBarcode(m.BarcodeId);
        }

        [RelayCommand]
        private async Task SaveEditAsync()
        {
            if (SelectedMember == null) return;
            SelectedMember.FirstName = FirstName;
            SelectedMember.LastName = LastName;
            SelectedMember.Email = Email;
            SelectedMember.PhoneNumber = Phone;
            await _svc.UpdateAsync(SelectedMember);
            StatusMessage = "Member updated.";
            await LoadMembersAsync();
        }

        [RelayCommand]
        private async Task DeactivateAsync()
        {
            if (SelectedMember == null) return;
            await _svc.DeactivateAsync(SelectedMember.MemberId);
            StatusMessage = $"{SelectedMember.FirstName} deactivated.";
            ClearForm();
            await LoadMembersAsync();
        }

        private void ClearForm()
        {
            FirstName = LastName = Email = Phone = "";
            SelectedMember = null;
        }
    }
}
