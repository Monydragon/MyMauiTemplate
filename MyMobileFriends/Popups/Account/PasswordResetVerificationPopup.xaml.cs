using Mopups.Interfaces;
using MyMobileFriends.Extensions;
using MyMobileFriends.API.Postmark;
using MyMobileFriends.Models;
using MyMobileFriends.Pages.Account;
using MyMobileFriends.Security;
using MyMobileFriends.Services.Interfaces;

namespace MyMobileFriends.Popups.Account;

public partial class PasswordResetVerificationPopup
{
    private readonly IUserService _userService;
    private readonly IPopupNavigation _popupNavigation;
    private readonly User? _currentUser;
    
    private CancellationTokenSource _cts = new();
    public PasswordResetVerificationPopup(IUserService userService, IPopupNavigation popupNavigation, User? user)
    {
        InitializeComponent();
        _userService = userService;
        _popupNavigation = popupNavigation;
        _currentUser = user;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await StartCountdownAsync(60, _cts.Token);
        
    }

    private async Task StartCountdownAsync(int seconds, CancellationToken cancellationToken)
    {
        try
        {
            for (var i = seconds; i >= 0; i--)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                await UpdateResendButtonTextAsync(i);

                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (TaskCanceledException)
        {
            // Handle the task cancellation here
            await UpdateResendButtonTextAsync(0);
        }
    }


    private async Task UpdateResendButtonTextAsync(int secondsRemaining)
    {
        // Execute the UI update on the main thread
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (resendVerificationButton == null)
                return;

            if (secondsRemaining > 0)
                resendVerificationButton.Text = $"Resend Verification Code ({secondsRemaining})";
            else
            {
                resendVerificationButton.Text = "Resend Verification Code";
                resendVerificationButton.IsEnabled = true;
            }
        });
    }


    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        if(_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        
        if(string.IsNullOrWhiteSpace(verificationEntry.Text))
        {
            await DisplayAlert("Error", "Please enter the verification code.", "OK");
            return;
        }
        
        if(await _userService.VerifyUser(_currentUser, verificationEntry.Text))
        {
            await DisplayAlert("Success", "Verification code accepted.", "OK");
            await Navigation.PushAsync(new PasswordResetPage(_userService, _popupNavigation, _currentUser));
            await _popupNavigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", "Verification code not accepted.", "OK");
        }
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        _cts.Cancel();
        await _popupNavigation.PopAsync();
    }

    private async void ResendVerificationButton_OnClicked(object? sender, EventArgs e)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        
        resendVerificationButton.IsEnabled = false;
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        
        var verificationCode = CodeGenerator.GenerateVerificationCode(DateTime.Now.AddHours(24));
        await _userService.SetUserVerificationCode(_currentUser, verificationCode);
        var response = await PostmarkApiController.Instance.SendEmailAsync(_currentUser.Email,EmailTemplate.CreatePasswordResetTemplate(_currentUser.Username, verificationCode.Code.DecryptString()));
        if (response.Status == PostmarkDotNet.PostmarkStatus.Success)
        {
            await DisplayAlert("Success", "Verification code sent.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Error sending verification code.", "OK");
        }
        
        await StartCountdownAsync(60, _cts.Token);
    }
    
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _cts.Cancel(); // Cancel the countdown when the popup is closed
    }
}