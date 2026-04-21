// ViewModels/UserManagementViewModel.cs
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
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

        // Egyptian mobile: 010, 011, 012, 015 followed by 8 digits
        private static readonly Regex EgyptPhoneRegex =
            new(@"^01[0125]\d{8}$", RegexOptions.Compiled);

        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

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
        [ObservableProperty] private string _statusColor = "#185FA5";

        // Tracks whether a member is loaded into the form for editing
        [ObservableProperty] private bool _isEditing = false;

        private List<Member> _allMembers = new();

        public UserManagementViewModel(
            MemberService svc, BarcodeService barcode, EncryptionService enc)
        {
            _svc = svc; _barcode = barcode; _enc = enc;
        }

        // Called when SearchText changes — filters the displayed list
        partial void OnSearchTextChanged(string value) => ApplyFilter();

        private void ApplyFilter()
        {
            var term = SearchText.Trim().ToLower();
            if (string.IsNullOrEmpty(term))
            {
                Members = new ObservableCollection<Member>(_allMembers);
                return;
            }
            var filtered = _allMembers.Where(m =>
                m.FirstName.ToLower().Contains(term) ||
                m.LastName.ToLower().Contains(term) ||
                m.BarcodeId.ToLower().Contains(term));
            Members = new ObservableCollection<Member>(filtered);
        }

        [RelayCommand]
        private async Task LoadMembersAsync()
        {
            _allMembers = await _svc.GetAllAsync();
            ApplyFilter();
        }

        [RelayCommand]
        private async Task AddMemberAsync()
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName))
            {
                SetStatus("First name and last name are required.", isError: true);
                return;
            }

            // Validate email if provided
            if (!string.IsNullOrWhiteSpace(Email) &&
                !EmailRegex.IsMatch(Email.Trim()))
            {
                SetStatus("Email address is not valid.", isError: true);
                return;
            }

            // Validate Egyptian phone if provided
            if (!string.IsNullOrWhiteSpace(Phone) &&
                !EgyptPhoneRegex.IsMatch(Phone.Trim()))
            {
                SetStatus("Phone must be an Egyptian number (e.g. 01012345678).", isError: true);
                return;
            }

            var m = new Member
            {
                FirstName = FirstName.Trim(),
                LastName = LastName.Trim(),
                Email = Email.Trim(),
                PhoneNumber = Phone.Trim(),
                IsActive = true
            };

            await _svc.AddAsync(m);
            BarcodeImage = _barcode.GenerateBarcode(m.BarcodeId);
            SetStatus($"Member {m.FirstName} added. Barcode: {m.BarcodeId}", isError: false);
            ResetFields();
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
            IsEditing = true;
            SetStatus($"Editing: {m.FirstName} {m.LastName}", isError: false);
        }

        [RelayCommand]
        private async Task SaveEditAsync()
        {
            if (SelectedMember == null)
            {
                SetStatus("No member selected. Click a row first.", isError: true);
                return;
            }

            // Validate email if provided
            if (!string.IsNullOrWhiteSpace(Email) &&
                !EmailRegex.IsMatch(Email.Trim()))
            {
                SetStatus("Email address is not valid.", isError: true);
                return;
            }

            // Validate Egyptian phone if provided
            if (!string.IsNullOrWhiteSpace(Phone) &&
                !EgyptPhoneRegex.IsMatch(Phone.Trim()))
            {
                SetStatus("Phone must be an Egyptian number (e.g. 01012345678).", isError: true);
                return;
            }

            SelectedMember.FirstName = FirstName.Trim();
            SelectedMember.LastName = LastName.Trim();
            SelectedMember.Email = Email.Trim();
            SelectedMember.PhoneNumber = Phone.Trim();

            await _svc.UpdateAsync(SelectedMember);
            SetStatus("Member updated successfully.", isError: false);
            IsEditing = false;
            ResetFields();
            await LoadMembersAsync();
        }

        [RelayCommand]
        private async Task DeactivateAsync()
        {
            if (SelectedMember == null) return;
            if (SelectedMember.IsActive == false)
            {
                SetStatus("Member is already inactive.", isError: true);
                return;
            }
            await _svc.DeactivateAsync(SelectedMember.MemberId);
            SetStatus($"{SelectedMember.FirstName} deactivated.", isError: false);
            ResetFields();
            await LoadMembersAsync();
        }

        [RelayCommand]
        private async Task ReactivateAsync()
        {
            if (SelectedMember == null) return;
            if (SelectedMember.IsActive == true)
            {
                SetStatus("Member is already active.", isError: true);
                return;
            }
            await _svc.ReactivateAsync(SelectedMember.MemberId);
            SetStatus($"{SelectedMember.FirstName} reactivated.", isError: false);
            ResetFields();
            await LoadMembersAsync();
        }

        [RelayCommand]
        public void ClearSelection()
        {
            ResetFields();
            SetStatus("", isError: false);
        }

        private void ResetFields()
        {
            FirstName = "";
            LastName = "";
            Email = "";
            Phone = "";
            SelectedMember = null;
            BarcodeImage = null;
            IsEditing = false;
        }

        private void SetStatus(string message, bool isError)
        {
            StatusMessage = message;
            StatusColor = isError ? "#A32D2D" : "#185FA5";
        }
    }
}