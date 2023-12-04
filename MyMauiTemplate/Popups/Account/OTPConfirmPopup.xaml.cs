using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Mopups.Interfaces;
using MyMauiTemplate.Core.Models;
using MyMauiTemplate.Core.Security;
using MyMauiTemplate.Core.Services.Interfaces;
using MyMauiTemplate.Pages.Account;
using MyMauiTemplate.Utilities;

namespace MyMauiTemplate.Popups.Account;

public partial class OtpConfirmPopup
{
    private readonly IUserService _userService;
    private readonly IPopupNavigation _popupNavigation;
    private readonly User? _currentUser;
    public OtpConfirmPopup(IUserService userService, IPopupNavigation popupNavigation, User? user)
    {
        InitializeComponent();
        _userService = userService;
        _popupNavigation = popupNavigation;
        _currentUser = user;
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        if(_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        
        if(_currentUser.TwoFactorSecret == null)
        {
            await DisplayAlert("Error", "No OTP secret found for this user.", "OK");
            return;
        }
        
        if(string.IsNullOrEmpty(otpEntry.Text))
        {
            await DisplayAlert("Error", "Please enter the OTP code.", "OK");
            return;
        }
        _currentUser.BackupCodes ??= new List<string>();
        
        var isValidOtp = TwoFactorAuthenticator.ValidateTotp(_currentUser, otpEntry.Text);
        if (isValidOtp.isValidated)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            IToast toast;
            if(isValidOtp.backupCodeUsed)
            {
                if(_currentUser.BackupCodes.Any())
                {
                    _currentUser.BackupCodes.Remove(otpEntry.Text);
                    toast = Toast.Make($"You are logged in as {_currentUser.Username} using a backup code this code was removed");
                }
                else
                {
                    _currentUser.TwoFactorSecret = null;
                    _currentUser.Require2Fa = false;
                    _currentUser.TwoFactorEnabled = false;
                    toast = Toast.Make($"You are logged in as {_currentUser.Username} using the last available backup code. Two-Factor Authentication (2FA) has now been disabled and all associated backup codes have been exhausted.");
                }
            }
            else
            {
                toast = Toast.Make($"You are logged in as {_currentUser.Username}");
            }
            await toast.Show(cancellationTokenSource.Token);

            var rememberMe = PreferencesHelper.GetRememberMe();
            _currentUser.StoreSavedPassword = rememberMe;
            if (rememberMe)
            {
                PreferencesHelper.SetLastStoredUsername(_currentUser.Username);
                PreferencesHelper.SetLastStoredPassword(_currentUser.Password);
            }
            _userService.CurrentUser = _currentUser;
            await _userService.SaveUserAsync(_currentUser);
            await Navigation.PushAsync(new ProfilePage(_userService, _popupNavigation, _currentUser));
            await _popupNavigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "Invalid OTP code.", "OK");
        }
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await _popupNavigation.PopAsync();
    }
}