using CommunityToolkit.Maui.Alerts;
using Mopups.Interfaces;
using MyMauiTemplate.Extensions;
using MyMauiTemplate.Models;
using MyMauiTemplate.Regex;
using MyMauiTemplate.Security;
using MyMauiTemplate.Services.Interfaces;
using MyMauiTemplate.Utilities;

namespace MyMauiTemplate.Pages.Account
{
    public partial class RegisterPage
    {
        private readonly IUserService _userService;
        private readonly IPopupNavigation _popupNavigation;
        private User? _currentUser;
        private string? _secretKey;

        public RegisterPage(IUserService userService, IPopupNavigation popupNavigation, User? user)
        {
            InitializeComponent();
            _userService = userService;
            _popupNavigation = popupNavigation;
            _currentUser = user ?? new User();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.CompletedTask;
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            var email = emailEntry.Text;
            var firstName = firstNameEntry.Text;
            var lastName = lastNameEntry.Text;
            var username = usernameEntry.Text;
            var password = passwordEntry.Text;
            var confirmPassword = confirmPasswordEntry.Text;
            var rememberMe = rememberMeSwitch.IsToggled;
            var trustedDevice = trustedDeviceSwitch.IsToggled;
            var autoLogin = autoLoginSwitch.IsToggled;
            var deviceId = $"{DeviceInfo.Name}_{DeviceInfo.Model}"; // Device identifier

            if(username.Length is < 3 or > 20)
            {
                await DisplayAlert("Error", "Username must be between 3 and 20 characters", "OK");
                return;
            }
            
            if (usernameEntry.Text.StartsWith("Guest", StringComparison.InvariantCultureIgnoreCase))
            {
                await DisplayAlert("Error", "Username cannot start with 'Guest'", "OK");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                await DisplayAlert("Error", "Please fill all required fields", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match", "OK");
                return;
            }

            if (!email.IsValidEmail())
            {
                await DisplayAlert("Error", "Invalid email format", "OK");
                return;
            }
            
            // Check for unique username and email
            var doesUsernameExist = await _userService.CheckUsernameExists(username);
            var doesEmailExist = await _userService.CheckEmailExists(email);
            
            if (doesUsernameExist)
            {
                await DisplayAlert("Error", "Username already taken", "OK");
                return;
            }
            
            if(doesEmailExist)
            {
                await DisplayAlert("Error", "Email already in use", "OK");
                return;
            }

            if (!RegexExpressions.PasswordRegex.IsMatch(password))
            {
                await DisplayAlert("Error", "Password does not meet the requirements\n" +
                                            "Password must contain a Minimum of eight characters, at least one uppercase letter, one lowercase letter, one number and one special character", "OK");
                return;
            }

            _currentUser ??= new User();
            if (_currentUser.IsGuest)
            {
                await _userService.MoveUserFiles(_currentUser.Username, username, _currentUser);
                _currentUser.IsGuest = false;
            }
            _currentUser.Email = email;
            _currentUser.FirstName = firstName;
            _currentUser.LastName = lastName;
            _currentUser.Username = username;
            _currentUser.Password = password.HashString();
            _currentUser.StoreSavedPassword = rememberMe;
            _currentUser.TrustedDevices ??= new List<string>();



            PreferencesHelper.SetRememberMe(rememberMe);
            if (rememberMe)
            {
                PreferencesHelper.SetLastStoredUsername(_currentUser.Username);
                PreferencesHelper.SetLastStoredPassword(_currentUser.Password);
                PreferencesHelper.SetAutoLogin(autoLogin);
                PreferencesHelper.SetTrustedDevice(trustedDevice);
                if (trustedDevice)
                {
                    if (!_currentUser.TrustedDevices.Contains(deviceId))
                    {
                        _currentUser.TrustedDevices.Add(deviceId);
                    }
                }
                else
                {
                    if (_currentUser.TrustedDevices.Contains(deviceId))
                    {
                        _currentUser.TrustedDevices.Remove(deviceId);
                    }
                }
            }

            await _userService.SaveUserAsync(_currentUser);
            
            var cancellationTokenSource = new CancellationTokenSource();
            var toast = Toast.Make($"User registered successfully! You are logged in as {_currentUser.Username}");
            await toast.Show(cancellationTokenSource.Token);
            _userService.CurrentUser = _currentUser;
            IoHelper.EnsureUserDirectoryExists(_currentUser);
            await Navigation.PushAsync(new ProfilePage(_userService, _popupNavigation, _currentUser));
        }

        private void OnPasswordRevealClicked(object sender, EventArgs e)
        {
            passwordEntry.IsPassword = !passwordEntry.IsPassword;
            passwordRevealButton.Text = passwordEntry.IsPassword ? "Show" : "Hide";
        }
        
        private void OnConfirmPasswordRevealClicked(object sender, EventArgs e)
        {
            confirmPasswordEntry.IsPassword = !confirmPasswordEntry.IsPassword;
            confirmPasswordRevealButton.Text = confirmPasswordEntry.IsPassword ? "Show" : "Hide";
        }
        
        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage(_userService, _popupNavigation));
        }

        private void RememberMeSwitch_OnToggled(object? sender, ToggledEventArgs e)
        {
            autoLoginSwitch.IsVisible = e.Value;
            autoLoginLabel.IsVisible = e.Value;
            trustedDeviceSwitch.IsVisible = e.Value;
            trustedDeviceLabel.IsVisible = e.Value;
        }

        private async void OnToggle2FAToggled(object sender, ToggledEventArgs e)
        {
            if (_currentUser == null)
            {
                await DisplayAlert("Error", "No user is currently logged in", "OK");
                return;
            }
            if (!e.Value && _currentUser.TwoFactorEnabled)
            {
                var confirmDisable = await DisplayAlert(
                    "Disable Two-Factor Authentication",
                    "Are you sure you want to disable Two-Factor Authentication? This will reset your 2FA setup, and you will need to configure it again to re-enable.",
                    "Yes, Disable", "Cancel");

                if (!confirmDisable)
                {
                    // Revert the toggle switch if user cancels
                    toggle2Fa.IsToggled = true;
                    return;
                }
            }
            
            if (e.Value)
            {
                // Generate a new secret key for 2FA
                _secretKey = string.IsNullOrWhiteSpace(_currentUser.TwoFactorSecret) 
                    ? TwoFactorAuthenticator.GenerateSecretKey() 
                    : _currentUser.TwoFactorSecret.DecryptString();
                
                // Generate and display the QR code
                var qrStream = TwoFaBarcodeGenerator.GenerateQrCodeStream(_secretKey, _currentUser.Username, "MyCharacterBuilder");
                qrCodeImage.Source = ImageSource.FromStream(() => qrStream);
                setupKeyDisplayLabel.Text = _secretKey;

                // Generate backup codes
                var backupCodes = _currentUser.BackupCodes;
                if(backupCodes == null || !backupCodes.Any())
                {
                    backupCodes = CodeGenerator.GenerateBackupCodes();
                    _currentUser.BackupCodes = backupCodes;
                }
                
                if (backupCodes.Count > 0)
                {
                    backupCodesDisplayLabel.Text = string.Join(Environment.NewLine, backupCodes.Select(code => code.DecryptString()));
                }

                use2FaOnTrustedDeviceSwitch.IsVisible = true;
                use2FaOnTrustedDeviceLabel.IsVisible = true;
                
                // Store the secret key and backup codes securely
                _currentUser.TwoFactorEnabled = true;
                _currentUser.TwoFactorSecret = _secretKey.EncryptString();
                _currentUser.BackupCodes = backupCodes;
                
                // Set QR code and backup codes list to visible
                qrCodeLabel.IsVisible = true;
                qrCodeImage.IsVisible = true;
                setupKeyDisplayLabel.IsVisible = true;
                backupCodesDisplayLabel.IsVisible = true;
                backupCodesLabel.IsVisible = true;
                setupKeyLabel.IsVisible = true;
                
            }
            else
            {
                // Hide QR code and backup codes list
                qrCodeImage.IsVisible = false;
                backupCodesDisplayLabel.IsVisible = false;
                setupKeyDisplayLabel.IsVisible = false;
                backupCodesLabel.IsVisible = false;
                setupKeyLabel.IsVisible = false;
                qrCodeLabel.IsVisible = false;
                
                use2FaOnTrustedDeviceSwitch.IsVisible = false;
                use2FaOnTrustedDeviceLabel.IsVisible = false;

                // Reset 2FA settings
                _currentUser.TwoFactorEnabled = false;
                _currentUser.TwoFactorSecret = null;
                _currentUser.BackupCodes = null;
            }

            // Save the updated user settings
            await _userService.SaveUserAsync(_currentUser);
        }

        private async void OnUse2FAOnTrustedDeviceToggled(object? sender, ToggledEventArgs e)
        {
            if (_currentUser == null)
            {
                await DisplayAlert("Error", "No user is currently logged in", "OK");
                return;
            }
            _currentUser.Require2Fa = e.Value;
            await _userService.SaveUserAsync(_currentUser);
        }

        private async void SetupKeyDisplayLabelTapGestureRecognizer_OnTapped(object sender, TappedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(setupKeyDisplayLabel.Text)) return;
            await Clipboard.SetTextAsync(setupKeyDisplayLabel.Text);
            await DisplayAlert("Copied", "Setup key copied to clipboard", "OK");
        }

        private async void BackupCodesDisplayLabelTapGestureRecognizer_OnTapped(object sender, TappedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(backupCodesDisplayLabel.Text)) return;
            await Clipboard.SetTextAsync(backupCodesDisplayLabel.Text);
            await DisplayAlert("Copied", "Backup Codes copied to clipboard", "OK");
        }
    }
}
