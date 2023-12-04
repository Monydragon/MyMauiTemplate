using Mopups.Interfaces;
using MyMauiTemplate.Core.API.Postmark;
using MyMauiTemplate.Core.Extensions;
using MyMauiTemplate.Core.Models;
using MyMauiTemplate.Core.Security;
using MyMauiTemplate.Core.Services.Interfaces;
using MyMauiTemplate.Popups.Account;

namespace MyMauiTemplate.Pages.Account;

public partial class PasswordRecoveryPage
{
    private readonly IUserService _userService;
    private readonly IPopupNavigation _popupNavigation;
    private User? _currentUser;
    
    public PasswordRecoveryPage(IUserService userService, IPopupNavigation popupNavigation, User? user)
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

    private async void OnSendVerificationClicked(object sender, EventArgs e)
    {
        var identifier = recoveryIdentifierEntry.Text;
        if (string.IsNullOrWhiteSpace(identifier))
        {
            await DisplayAlert("Error", "Please enter your username or email address.", "OK");
            return;
        }
        var user = await _userService.GetUserByEmailOrUsername(identifier);
        if (user == null)
        {
            await DisplayAlert("Error", "No user found with that username or email address.", "OK");
            return;
        }
        _currentUser = user;
        var verificationCode = CodeGenerator.GenerateVerificationCode(DateTime.Now.AddHours(24));
        await _userService.SetUserVerificationCode(_currentUser, verificationCode);
        var response = await PostmarkApiController.Instance.SendEmailAsync(_currentUser.Email,EmailTemplate.CreatePasswordResetTemplate(user.Username, verificationCode.Code.DecryptString()));
        if (response.Status == PostmarkDotNet.PostmarkStatus.Success)
        {
            await DisplayAlert("Success", "Verification code sent.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Error sending verification code.", "OK");
        }
        await _popupNavigation.PushAsync(new PasswordResetVerificationPopup(_userService, _popupNavigation, _currentUser));
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage(_userService, _popupNavigation));
    }
}