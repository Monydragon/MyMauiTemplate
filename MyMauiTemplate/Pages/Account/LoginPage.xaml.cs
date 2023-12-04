using CommunityToolkit.Maui.Alerts;
using Mopups.Interfaces;
using MyMauiTemplate.Core.Models;
using MyMauiTemplate.Core.Services.Interfaces;
using MyMauiTemplate.Core.Utilities;
using MyMauiTemplate.Popups.Account;
using MyMauiTemplate.Utilities;

namespace MyMauiTemplate.Pages.Account;

public partial class LoginPage
{
    private readonly IUserService _userService;
    private readonly IPopupNavigation _popupNavigation;
    private User? _currentUser;
    public LoginPage(IUserService userService, IPopupNavigation popupNavigation)
    {
        InitializeComponent();
        _userService = userService;
        _popupNavigation = popupNavigation;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        rememberMeSwitch.IsToggled = PreferencesHelper.GetRememberMe();
        autoLoginSwitch.IsToggled = PreferencesHelper.GetAutoLogin();
        if (!rememberMeSwitch.IsToggled) return;
        var lastStoredID = PreferencesHelper.GetLastStoredUserId();
        var lastStoredUsername = PreferencesHelper.GetLastStoredUsername();
        var lastStoredPassword = PreferencesHelper.GetLastStoredPassword();
        if (lastStoredID != Guid.Empty)
        {
            _currentUser = await _userService.GetUserById(lastStoredID);
        }

        _currentUser ??= await _userService.GetUserByUsername(lastStoredUsername);
        if (_currentUser == null) return;
        
        if(!string.IsNullOrWhiteSpace(lastStoredUsername))
        {
            usernameEntry.Text = lastStoredUsername;
        }
        
        if(!string.IsNullOrWhiteSpace(lastStoredPassword))
        {
            passwordEntry.Text = lastStoredPassword;
        }

        if (!autoLoginSwitch.IsToggled) return;
        
        await Login(_currentUser);
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var username = usernameEntry.Text;
        var password = passwordEntry.Text;
        var id = Guid.Empty;

        if (!string.IsNullOrEmpty(username))
        {
            if (IsGuestLogin(username))
            {
                var guestUser = await _userService.GetGuestUser();
                if (guestUser != null)
                {
                    await Login(guestUser);
                    return;
                }
            }
        }

        // Check for Remember Me and Trusted Device
        var rememberMe = PreferencesHelper.GetRememberMe();
        if (rememberMe)
        {
            id = PreferencesHelper.GetLastStoredUserId();
        }
        var trustedDevice = PreferencesHelper.GetTrustedDevice();
        var deviceId = $"{DeviceInfo.Name}_{DeviceInfo.Model}"; // Device identifier
        
        User? authenticatedUser;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please enter both username and password.", "OK");
            return;
        }

        if (rememberMe)
        {
            authenticatedUser = await _userService.GetUserById(id) ?? await _userService.GetUserByUsername(username);
        }
        else
        {
            authenticatedUser = await _userService.AuthenticateUser(username, password);
        }
        
        _currentUser = authenticatedUser;
        
        if (authenticatedUser == null)
        {
            await DisplayAlert("Error", "Invalid username or password.", "OK");
            return;
        }
        
        authenticatedUser.BackupCodes ??= new List<string>();
        authenticatedUser.TrustedDevices ??= new List<string>();
        
        switch (authenticatedUser.TwoFactorEnabled)
        {
            // Standard 2FA check
            case true:
                if (trustedDevice && authenticatedUser.TrustedDevices.Contains(deviceId) && !authenticatedUser.Require2Fa)
                {
                    // Auto-login
                    await LoginSuccess(authenticatedUser);
                    return;
                }
                ShowTwoFactorAuthPopup();
                break;
            default:
                if (rememberMe && !string.IsNullOrEmpty(username))
                {
                    if (_userService.Users is { Count: > 0 })
                    {
                        authenticatedUser = await _userService.GetUserByUsername(username);
                    }
            
                    if (authenticatedUser != null)
                    {
                        // Auto-login
                        await LoginSuccess(authenticatedUser);
                    }
                }
                break;
        }
    }
    
    private async Task Login(User user)
    {
        _currentUser = user;
        _userService.CurrentUser = _currentUser;
        
        if (rememberMeSwitch.IsToggled)
        {
            PreferencesHelper.SetLastStoredUserId(_currentUser.Id);
            PreferencesHelper.SetLastStoredUsername(_currentUser.Username);
            PreferencesHelper.SetLastStoredPassword(_currentUser.Password);
            _currentUser.StoreSavedPassword = rememberMeSwitch.IsToggled;
        }
        
        IoHelper.EnsureUserDirectoryExists(_currentUser);
        await Navigation.PushAsync(new ProfilePage(_userService, _popupNavigation, _currentUser));
    }

    private async Task LoginSuccess(User user)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var toast = Toast.Make($"You are logged in as {user.Username}");
        await toast.Show(cancellationTokenSource.Token);
        await Login(user);
    }

    private void OnPasswordRevealClicked(object sender, EventArgs e)
    {
        passwordEntry.IsPassword = !passwordEntry.IsPassword;
        passwordRevealButton.Text = passwordEntry.IsPassword ? "Show" : "Hide";
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_userService, _popupNavigation, null));
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage(_userService, _popupNavigation));
    }

    private async void ShowTwoFactorAuthPopup()
    {
        await _popupNavigation.PushAsync(new OtpConfirmPopup(_userService, _popupNavigation, _currentUser));
    }

    private void RememberMeSwitch_OnToggled(object? sender, ToggledEventArgs e)
    {
        PreferencesHelper.SetRememberMe(e.Value);
        autoLoginLabel.IsVisible = e.Value;
        autoLoginSwitch.IsVisible = e.Value;

        if (e.Value) return;
        autoLoginSwitch.IsToggled = false;
        PreferencesHelper.SetAutoLogin(false);
    }

    private void AutoLoginSwitch_OnToggled(object? sender, ToggledEventArgs e)
    {
        PreferencesHelper.SetAutoLogin(e.Value);
    }

    private async void OnForgotPasswordClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new PasswordRecoveryPage(_userService, _popupNavigation, _currentUser));
    }

    private async void OnGuestLoginClicked(object sender, EventArgs e)
    {
        var existingGuestUser = await _userService.GetGuestUser();
        if (existingGuestUser != null)
        {
            await Login(existingGuestUser);
            return;
        }
        
        // Create a new guest user instance
        var guestUser = new User
        {
            IsGuest = true
        };

        // Set the Username to "Guest - " followed by the User's Id
        guestUser.Username = $"Guest - {guestUser.Id}";
        
        _currentUser = guestUser;
        // Perform necessary actions for a guest user
        _userService.CurrentUser = _currentUser;
        await Login(_currentUser);
    }
    
    private bool IsGuestLogin(string username)
    {
        return username.StartsWith("Guest", StringComparison.InvariantCultureIgnoreCase);
    }
}