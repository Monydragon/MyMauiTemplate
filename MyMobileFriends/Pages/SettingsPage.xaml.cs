using Mopups.Interfaces;
using MyMauiTemplate.Services.Interfaces;
using MyMauiTemplate.Utilities;

namespace MyMauiTemplate.Pages;

public partial class SettingsPage
{
    private IUserService _userService;
    private IPopupNavigation _popupNavigation;
    public SettingsPage(IUserService userService, IPopupNavigation popupNavigation)
    {
        InitializeComponent();
        SetInitialThemeSelection();
        themePicker.SelectedIndexChanged += OnThemePickerSelectedIndexChanged;
        _userService = userService;
        _popupNavigation = popupNavigation;
    }

    private void SetInitialThemeSelection()
    {
        var savedTheme = PreferencesHelper.GetTheme();
        themePicker.SelectedIndex = savedTheme switch
        {
            AppTheme.Unspecified => 0,
            AppTheme.Light => 1,
            AppTheme.Dark => 2,
            _ => 0
        };
    }

    private void OnThemePickerSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (Application.Current == null) return;
        var selectedTheme = themePicker.SelectedItem.ToString();

        switch (selectedTheme)
        {
            case "System":
                Application.Current.UserAppTheme = AppTheme.Unspecified;
                break;
            case "Light":
                Application.Current.UserAppTheme = AppTheme.Light;
                break;
            case "Dark":
                Application.Current.UserAppTheme = AppTheme.Dark;
                break;
        }

        if(!string.IsNullOrWhiteSpace(selectedTheme))
        {
            Preferences.Set("AppThemePreference", selectedTheme);
        }
    }

}