using Mopups.Interfaces;
using MyMobileFriends.Extensions;
using MyMobileFriends.Models;
using MyMobileFriends.Regex;
using MyMobileFriends.Services.Interfaces;

namespace MyMobileFriends.Pages.Account;

public partial class PasswordResetPage
{
    private readonly IUserService _userService;
    private readonly IPopupNavigation _popupNavigation;
    private readonly User? _currentUser;
    public PasswordResetPage(IUserService userService, IPopupNavigation popupNavigation, User? user)
    {
        InitializeComponent();
        _userService = userService;
        _popupNavigation = popupNavigation;
        _currentUser = user;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.CompletedTask;
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage(_userService, _popupNavigation));
    }

    private async void OnChangePasswordClicked(object sender, EventArgs e)
    {
        if (_currentUser != null)
        {
            var newPassword = newPasswordEntry.Text;
            var confirmNewPassword = confirmNewPasswordEntry.Text;


            if (_currentUser == null)
            {
                await DisplayAlert("Error", "No user is currently logged in", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmNewPassword))
            {
                await DisplayAlert("Error", "Please enter all fields", "OK");
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
            _userService.CurrentUser = _currentUser;
            // Update the user's password in the data store
            await _userService.SaveUserAsync(_currentUser);

            await DisplayAlert("Success", "Password changed successfully", "OK");
            await Navigation.PushAsync(new ProfilePage(_userService, _popupNavigation, _currentUser));
        }
        else
        {
            await DisplayAlert("Error", "You must be logged in to change your password.", "OK");
        }
    }

    private async void OnNewPasswordRevealClicked(object sender, EventArgs e)
    {
        newPasswordEntry.IsPassword = !newPasswordEntry.IsPassword;
        newPasswordRevealButton.Text = newPasswordEntry.IsPassword ? "Show" : "Hide";
        await Task.CompletedTask;
    }

    private async void OnConfirmNewPasswordRevealClicked(object sender, EventArgs e)
    {
        confirmNewPasswordEntry.IsPassword = !confirmNewPasswordEntry.IsPassword;
        confirmNewPasswordRevealButton.Text = confirmNewPasswordEntry.IsPassword ? "Show" : "Hide";
        await Task.CompletedTask;
    }
}