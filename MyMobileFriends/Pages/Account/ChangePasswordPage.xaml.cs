using Mopups.Interfaces;
using MyMobileFriends.Extensions;
using MyMobileFriends.Models;
using MyMobileFriends.Regex;
using MyMobileFriends.Services.Interfaces;

namespace MyMobileFriends.Pages.Account;

public partial class ChangePasswordPage
{
    private readonly IUserService _userService;
    private readonly IPopupNavigation _popupNavigation;
    private User? _currentUser;
    public ChangePasswordPage(IUserService userService, IPopupNavigation popupNavigation, User? user)
    {
        InitializeComponent();
        _userService = userService;
        _popupNavigation = popupNavigation;
        _currentUser = user;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _currentUser = _userService.CurrentUser;
        await Task.CompletedTask;
    }

    private async void OnChangePasswordClicked(object sender, EventArgs e)
    {
        var currentPassword = currentPasswordEntry.Text;
        var newPassword = newPasswordEntry.Text;
        var confirmNewPassword = confirmNewPasswordEntry.Text;
        
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmNewPassword))
        {
            await DisplayAlert("Error", "Please enter all fields", "OK");
            return;
        }

        if (!_currentUser.Password.VerifyMatchingHash(currentPassword))
        {
            await DisplayAlert("Error", "Current password is incorrect", "OK");
            return;
        }

        if (newPassword != confirmNewPassword)
        {
            await DisplayAlert("Error", "New passwords do not match", "OK");
            return;
        }

        if (!RegexExpressions.PasswordRegex.IsMatch(newPassword))
        {
            await DisplayAlert("Error", "Password does not meet the requirements\n" +
                                        "Password must contain a Minimum of eight characters, at least one uppercase letter, one lowercase letter, one number and one special character", "OK");
            return;
        }

        _currentUser.Password = newPassword.HashString();
        // Update the user's password in the data store
        await _userService.SaveUserAsync(_currentUser);

        await DisplayAlert("Success", "Password changed successfully", "OK");
        await Navigation.PopAsync();
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage(_userService, _popupNavigation));
    }

    private void OnCurrentPasswordRevealClicked(object sender, EventArgs e)
    {
        currentPasswordEntry.IsPassword = !currentPasswordEntry.IsPassword;
        currentPasswordRevealButton.Text = currentPasswordEntry.IsPassword ? "Show" : "Hide";
    }

    private void OnNewPasswordRevealClicked(object sender, EventArgs e)
    {
        newPasswordEntry.IsPassword = !newPasswordEntry.IsPassword;
        newPasswordRevealButton.Text = newPasswordEntry.IsPassword ? "Show" : "Hide";
        
    }

    private void OnConfirmNewPasswordRevealClicked(object sender, EventArgs e)
    {
        confirmNewPasswordEntry.IsPassword = !confirmNewPasswordEntry.IsPassword;
        confirmNewPasswordRevealButton.Text = confirmNewPasswordEntry.IsPassword ? "Show" : "Hide";
    }
}