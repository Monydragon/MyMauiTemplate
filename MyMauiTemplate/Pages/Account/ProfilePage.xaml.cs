using Mopups.Interfaces;
using MyMauiTemplate.Extensions;
using MyMauiTemplate.Models;
using MyMauiTemplate.Security;
using MyMauiTemplate.Services.Interfaces;
using MyMauiTemplate.Utilities;

namespace MyMauiTemplate.Pages.Account;

public partial class ProfilePage
{
    private readonly IUserService _userService;
    private readonly IPopupNavigation _popupNavigation;
    private User? _currentUser;
    private string? _initialUsername;
    private string? _initialFirstName;
    private string? _initialLastName;
    private string? _initialEmail;
    private string? _secretKey;


    public ProfilePage(IUserService userService, IPopupNavigation popupNavigation, User? user)
    {
        InitializeComponent();
        _userService = userService;
        _popupNavigation = popupNavigation;
        _currentUser = user;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }

        createAccountButton.IsVisible = _currentUser.IsGuest;
        // Check if the current user is a guest
        if (_currentUser.IsGuest)
        {
            await ShowGuestElements();
        }
        else
        {
            await BindUserData();
        }
    }

    private async void OnUploadPictureClicked(object sender, EventArgs e)
    {
        try
        {
            if (_currentUser == null)
            {
                await DisplayAlert("Error", "No user is currently logged in", "OK");
                return;
            }
            
            // Use File Picker to select an image
            var pickResult = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a profile picture",
                FileTypes = FilePickerFileType.Images,
            });

            if (pickResult == null) return;
            var fileStream = await pickResult.OpenReadAsync();
            var fileExtension = pickResult.FileName.Split('.').LastOrDefault();
            if (fileExtension == null)
            {
                await DisplayAlert("Error", "Invalid file type", "OK");
                return;
            }
            
            var profilePicturePath = AppPaths.GetProfilePicturePath(_currentUser.Username, fileExtension);

            // Save the file to the profile picture path
            await using (var file = File.OpenWrite(profilePicturePath))
            {
                await fileStream.CopyToAsync(file);
            }

            // Update user's profile picture path
            _currentUser.ProfilePicturePath = profilePicturePath;

            // Refresh the profile picture on the UI
            profileImage.Source = ImageSource.FromStream(() => new FileStream(profilePicturePath, FileMode.Open, FileAccess.Read));

            // Optionally, save the user data if needed
            await _userService.SaveUserAsync(_currentUser);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred while uploading the picture: {ex.Message}", "OK");
        }
    }
    
    private async Task ToggleEditMode(Entry entryField, Button editButton)
    {
        
        if (entryField.IsReadOnly)
        {
            // Store the initial value when entering edit mode
            await StoreInitialValue(entryField);

            entryField.IsReadOnly = false;
            entryField.Focus(); // Set focus and select all text
            entryField.CursorPosition = 0;
            entryField.SelectionLength = entryField.Text?.Length ?? 0;
            editButton.Text = "Save";
        }
        else
        {
            await SaveOrRevertChanges(entryField, editButton);
        }
    }
    
    private async Task SaveOrRevertChanges(Entry entryField, Button editButton)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        
        if (await IsValidUserData())
        {
            entryField.IsReadOnly = true;
            editButton.Text = "Edit";
            await UpdateCurrentUser(entryField);
            await _userService.SaveUserAsync(_currentUser);
        }
        else
        {
            // Revert to initial values if validation fails
            await RevertToInitialValue(entryField);
        }
    }
    
    private async Task StoreInitialValue(Entry entryField)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        
        switch (entryField)
        {
            case var _ when entryField == usernameEntry:
                _initialUsername = _currentUser.Username;
                break;
            case var _ when entryField == firstNameEntry:
                _initialFirstName = _currentUser.FirstName;
                break;
            case var _ when entryField == lastNameEntry:
                _initialLastName = _currentUser.LastName;
                break;
            case var _ when entryField == emailEntry:
                _initialEmail = _currentUser.Email;
                break;
        }
    }

    private async Task RevertToInitialValue(Entry entryField)
    {
        switch (entryField)
        {
            case var _ when entryField == usernameEntry:
                entryField.Text = _initialUsername;
                break;
            case var _ when entryField == firstNameEntry:
                entryField.Text = _initialFirstName;
                break;
            case var _ when entryField == lastNameEntry:
                entryField.Text = _initialLastName;
                break;
            case var _ when entryField == emailEntry:
                entryField.Text = _initialEmail;
                break;
        }
        
        await Task.CompletedTask;
    }

    private async Task UpdateCurrentUser(Entry entryField)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        
        switch (entryField)
        {
            case var _ when entryField == usernameEntry:
                _currentUser.Username = entryField.Text;
                break;
            case var _ when entryField == firstNameEntry:
                _currentUser.FirstName = entryField.Text;
                break;
            case var _ when entryField == lastNameEntry:
                _currentUser.LastName = entryField.Text;
                break;
            case var _ when entryField == emailEntry:
                _currentUser.Email = entryField.Text;
                break;
        }
        
        await Task.CompletedTask;
    }
    
    private async Task<bool> IsValidUserData()
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(usernameEntry.Text) ||
            string.IsNullOrWhiteSpace(emailEntry.Text))
        {
            await DisplayAlert("Error", "Please fill all required fields", "OK");
            return false;
        }

        if (!emailEntry.Text.IsValidEmail())
        {
            await DisplayAlert("Error", "Invalid email format", "OK");
            return false;
        }
        
        // Check for unique username
        var doesUsernameExist = await _userService.CheckUsernameExists(usernameEntry.Text);
        var doesEmailExist = await _userService.CheckEmailExists(emailEntry.Text);
        
        if(doesUsernameExist && !usernameEntry.Text.Equals(_currentUser.Username, StringComparison.InvariantCultureIgnoreCase))
        {
            await DisplayAlert("Error", "Username already taken", "OK");
            return false;
        }
        
        if(usernameEntry.Text.Length is < 3 or > 20)
        {
            await DisplayAlert("Error", "Username must be between 3 and 20 characters", "OK");
            return false;
        }

        if (usernameEntry.Text.StartsWith("Guest", StringComparison.InvariantCultureIgnoreCase))
        {
            await DisplayAlert("Error", "Username cannot start with 'Guest'", "OK");
        }
        
        if(doesEmailExist && !emailEntry.Text.Equals(_currentUser.Email, StringComparison.InvariantCultureIgnoreCase))
        {
            await DisplayAlert("Error", "Email already in use", "OK");
            return false;
        }

        return true;
    }

    private async void OnChangePasswordButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChangePasswordPage(_userService, _popupNavigation, _currentUser));
    }

    private async void OnEditUsernameClicked(object sender, EventArgs e)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        
        await ToggleEditMode(usernameEntry, (Button)sender);
        _currentUser.Username = usernameEntry.Text;
    }

    private async void OnEditFirstNameClicked(object sender, EventArgs e)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        await ToggleEditMode(firstNameEntry, (Button)sender);
        _currentUser.FirstName = firstNameEntry.Text;
    }

    private async void OnEditLastNameClicked(object sender, EventArgs e)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        await ToggleEditMode(lastNameEntry, (Button)sender);
        _currentUser.LastName = lastNameEntry.Text;
    }

    private async void OnEditEmailClicked(object sender, EventArgs e)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        await ToggleEditMode(emailEntry, (Button)sender);
        _currentUser.Email = emailEntry.Text;
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
       await Navigation.PushAsync(new SettingsPage(_userService, _popupNavigation));
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
            show2FaInfoButton.IsVisible = true;
            
            // Store the secret key and backup codes securely
            _currentUser.TwoFactorEnabled = true;
            _currentUser.TwoFactorSecret = _secretKey.EncryptString();
            _currentUser.BackupCodes = backupCodes;
            
            // Logic to enable 2FA...

            show2FaInfoButton.IsVisible = true;
            show2FaInfoButton.Text = "Hide 2FA Information";
            
            // Set QR code and backup codes list to visible
            qrCodeLabel.IsVisible = true;
            qrCodeImage.IsVisible = true;
            setupKeyDisplayLabel.IsVisible = true;
            backupCodesDisplayLabel.IsVisible = true;
            backupCodesLabel.IsVisible = true;
            backupCodesGenerateButton.IsVisible = true;
            setupKeyLabel.IsVisible = true;
        }
        else
        {
            // Hide QR code and backup codes list
            qrCodeImage.IsVisible = false;
            backupCodesDisplayLabel.IsVisible = false;
            show2FaInfoButton.IsVisible = false;
            setupKeyDisplayLabel.IsVisible = false;
            backupCodesLabel.IsVisible = false;
            setupKeyLabel.IsVisible = false;
            qrCodeLabel.IsVisible = false;
            backupCodesGenerateButton.IsVisible = false;
            use2FaOnTrustedDeviceSwitch.IsVisible = false;
            use2FaOnTrustedDeviceLabel.IsVisible = false;

            show2FaInfoButton.Text = "Show 2FA Information";


            // Reset 2FA settings
            _currentUser.TwoFactorEnabled = false;
            _currentUser.TwoFactorSecret = null;
            _currentUser.BackupCodes = null;
        }

        // Save the updated user settings
        await _userService.SaveUserAsync(_currentUser);
    }

    private async void Show2FAInfo_OnClicked(object sender, EventArgs e)
    {
        var isInfoVisible = qrCodeImage.IsVisible;
        qrCodeLabel.IsVisible = !isInfoVisible;
        qrCodeImage.IsVisible = !isInfoVisible;
        setupKeyDisplayLabel.IsVisible = !isInfoVisible;
        backupCodesDisplayLabel.IsVisible = !isInfoVisible;
        backupCodesLabel.IsVisible = !isInfoVisible;
        backupCodesGenerateButton.IsVisible = !isInfoVisible;
        setupKeyLabel.IsVisible = !isInfoVisible;
        show2FaInfoButton.Text = isInfoVisible ? "Show 2FA Information" : "Hide 2FA Information";

        await Task.CompletedTask;
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        _currentUser = null;
        // Perform logout operations here
        _userService.CurrentUser = _currentUser;
        PreferencesHelper.ClearPreferences("AutoLogin");
        
        // Navigate to the login page or another appropriate page
        await Navigation.PushAsync(new LoginPage(_userService, _popupNavigation));    
    }

    private async void AutoLoginSwitch_OnToggled(object sender, ToggledEventArgs e)
    {
        PreferencesHelper.SetAutoLogin(e.Value);
        await Task.CompletedTask;
    }

    private async void OnModifyTrustedDevicesClicked(object sender, EventArgs e)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        
        _currentUser.TrustedDevices ??= new List<string>();
        
        var deviceName = $"{DeviceInfo.Name}_{DeviceInfo.Model}";
        trustedDevicesLabel.IsVisible = !trustedDevicesLabel.IsVisible;
        trustedDevicesList.IsVisible = !trustedDevicesList.IsVisible;
        addTrustedDeviceButton.IsVisible = _currentUser.TrustedDevices.All(x => x != deviceName);
        modifyTrustedDevicesButton.Text = trustedDevicesLabel.IsVisible ? "Hide Trusted Devices" : "Modify Trusted Devices";
        
        await Task.CompletedTask;
    }

    private async void OnAddCurrentDeviceClicked(object sender, EventArgs e)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }

        _currentUser.TrustedDevices ??= new List<string>();
        var deviceName = $"{DeviceInfo.Name}_{DeviceInfo.Model}";
        if(_currentUser.TrustedDevices.Contains(deviceName)) return;
        _currentUser.TrustedDevices.Add(deviceName);
        trustedDevicesList.ItemsSource = null;
        trustedDevicesList.ItemsSource = _currentUser.TrustedDevices;
        addTrustedDeviceButton.IsVisible = false;
        PreferencesHelper.SetTrustedDevice(true);
        await _userService.SaveUserAsync(_currentUser);
    }
    
    private async void OnRemoveTrustedDeviceClicked(object sender, EventArgs e)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        _currentUser.TrustedDevices ??= new List<string>();
        var deviceName = $"{DeviceInfo.Name}_{DeviceInfo.Model}";
        var button = (Button)sender;
        var device = button.CommandParameter as string;
        if (device == null || !_currentUser.TrustedDevices.Contains(device)) return;
        _currentUser.TrustedDevices.Remove(device);
        trustedDevicesList.ItemsSource = null;
        trustedDevicesList.ItemsSource = _currentUser.TrustedDevices;
        addTrustedDeviceButton.IsVisible = true;
        if (device.Equals(deviceName))
        {
            PreferencesHelper.SetTrustedDevice(false);
        }
        await _userService.SaveUserAsync(_currentUser);
    }

    private async void OnUse2FAOnTrustedDeviceToggled(object sender, ToggledEventArgs e)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        _currentUser.Require2Fa = e.Value;
        await _userService.SaveUserAsync(_currentUser);
    }

    private async void RememberMeSwitch_OnToggled(object sender, ToggledEventArgs e)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        _currentUser.StoreSavedPassword = e.Value;
        autoLoginLabel.IsVisible = e.Value;
        autoLoginSwitch.IsVisible = e.Value;
        PreferencesHelper.SetRememberMe(e.Value);

        if (!e.Value)
        {
            autoLoginSwitch.IsToggled = false;
            PreferencesHelper.SetAutoLogin(false);
        }
        
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

    private async void OnDeleteAccountClicked(object sender, EventArgs e)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        
        var confirmDeleteTitle = _currentUser.IsGuest ? "Delete Guest Account" : "Delete Account";
        var confirmDeleteMessage = _currentUser.IsGuest
            ? "Are you sure you want to delete your guest account? This action cannot be undone."
            : "Are you sure you want to delete your account? This action cannot be undone.";
        
        var confirmDelete = await DisplayAlert(
            confirmDeleteTitle,
            confirmDeleteMessage,
            "Delete", "Cancel");

        if (!confirmDelete) return;

        await _userService.DeleteUserAsync(_currentUser);
        
        // Clear the preferences
        PreferencesHelper.ClearPreferences("AutoLogin");
        PreferencesHelper.ClearPreferences("RememberMe");
        PreferencesHelper.ClearPreferences("TrustedDevice");
        // Navigate to the login page
        await Navigation.PushAsync(new LoginPage(_userService, _popupNavigation));
    }

    private async void OnCreateAccountClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_userService, _popupNavigation, _currentUser));
    }
    
    private async Task ShowGuestElements()
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        // Hide UI elements that are not relevant for guest users

        usernameLabel.IsVisible = false;
        usernameEntry.IsVisible = false;
        editUsernameButton.IsVisible = false;
        firstNameLabel.IsVisible = false;
        firstNameEntry.IsVisible = false;
        editFirstNameButton.IsVisible = false;
        lastNameLabel.IsVisible = false;
        lastNameEntry.IsVisible = false;
        editLastNameButton.IsVisible = false;
        emailLabel.IsVisible = false;
        emailEntry.IsVisible = false;
        editEmailButton.IsVisible = false;
        toggle2FaLabel.IsVisible = false;
        toggle2Fa.IsVisible = false;
        use2FaOnTrustedDeviceLabel.IsVisible = false;
        use2FaOnTrustedDeviceSwitch.IsVisible = false;
        modifyTrustedDevicesButton.IsVisible = false;
        trustedDevicesList.IsVisible = false;
        addTrustedDeviceButton.IsVisible = false;
        qrCodeImage.IsVisible = false;
        setupKeyDisplayLabel.IsVisible = false;
        backupCodesDisplayLabel.IsVisible = false;
        show2FaInfoButton.IsVisible = false;
        changePasswordButton.IsVisible = false;
        
        // Keep "Remember Me" and "Auto Login" visible for guest users
        rememberMeLabel.IsVisible = true;
        rememberMeSwitch.IsVisible = true;
        autoLoginLabel.IsVisible = true;
        autoLoginSwitch.IsVisible = true;
        
        // Keep profile image visible for guest users
        profileImageLabel.IsVisible = true;
        profileImage.IsVisible = true;
        profileImage.Source = string.IsNullOrWhiteSpace(_currentUser.ProfilePicturePath)
            ? "default_profile.png"
            : _currentUser.ProfilePicturePath;
        
        deleteAccountButton.Text = "Delete Guest Account";
        deleteAccountButton.IsVisible = true;
        
        rememberMeSwitch.IsToggled = PreferencesHelper.GetRememberMe();
        autoLoginSwitch.IsToggled = PreferencesHelper.GetAutoLogin();
    }

    private async Task BindUserData()
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }
        
        toggle2Fa.IsToggled = _currentUser.TwoFactorEnabled;

        // Make sure all UI elements are visible
        profileImageLabel.IsVisible = true;
        profileImage.IsVisible = true;
        usernameLabel.IsVisible = true;
        usernameEntry.IsVisible = true;
        editUsernameButton.IsVisible = true;
        firstNameLabel.IsVisible = true;
        firstNameEntry.IsVisible = true;
        editFirstNameButton.IsVisible = true;
        lastNameLabel.IsVisible = true;
        lastNameEntry.IsVisible = true;
        editLastNameButton.IsVisible = true;
        emailLabel.IsVisible = true;
        emailEntry.IsVisible = true;
        editEmailButton.IsVisible = true;
        rememberMeLabel.IsVisible = true;
        rememberMeSwitch.IsVisible = true;
        autoLoginLabel.IsVisible = true;
        autoLoginSwitch.IsVisible = true;
        toggle2FaLabel.IsVisible = true;
        toggle2Fa.IsVisible = true;
        show2FaInfoButton.IsVisible = toggle2Fa.IsToggled;
        use2FaOnTrustedDeviceLabel.IsVisible = toggle2Fa.IsToggled;
        use2FaOnTrustedDeviceSwitch.IsVisible = toggle2Fa.IsToggled;
        changePasswordButton.IsVisible = true;
        deleteAccountButton.IsVisible = true;

        // Populate the UI elements with the user data
        usernameEntry.Text = _currentUser.Username;
        firstNameEntry.Text = _currentUser.FirstName;
        lastNameEntry.Text = _currentUser.LastName;
        emailEntry.Text = _currentUser.Email;
        profileImage.Source = string.IsNullOrWhiteSpace(_currentUser.ProfilePicturePath)
            ? "default_profile.png"
            : _currentUser.ProfilePicturePath;

        // Set the state of switches and other elements as per user's settings
        rememberMeSwitch.IsToggled = PreferencesHelper.GetRememberMe();
        autoLoginSwitch.IsToggled = PreferencesHelper.GetAutoLogin();
        use2FaOnTrustedDeviceSwitch.IsToggled = _currentUser.Require2Fa;
        trustedDevicesList.ItemsSource = _currentUser.TrustedDevices;
        deleteAccountButton.Text = "Delete Account";
        
        rememberMeSwitch.IsToggled = PreferencesHelper.GetRememberMe();
        autoLoginSwitch.IsToggled = PreferencesHelper.GetAutoLogin();

    }

    private async void OnGenerateBackupCodesClicked(object? sender, EventArgs e)
    {
        if (_currentUser == null)
        {
            await DisplayAlert("Error", "No user is currently logged in", "OK");
            return;
        }

        if (_currentUser.TwoFactorSecret == null)
        {
            await DisplayAlert("Error", "No OTP secret found for this user.", "OK");
            return;
        }

        var backupCodes = CodeGenerator.GenerateBackupCodes();
        if (backupCodes.Count > 0)
        {
            backupCodesDisplayLabel.Text = string.Join(Environment.NewLine, backupCodes.Select(code => code.DecryptString()));
        }
        _currentUser.BackupCodes = backupCodes;
        await _userService.SaveUserAsync(_currentUser);
    }
}