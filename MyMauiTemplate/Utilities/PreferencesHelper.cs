namespace MyMauiTemplate.Utilities;

public static class PreferencesHelper
{
    public static bool ClearPreferences()
    {
        try
        {
            Preferences.Clear();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public static bool ClearPreferences(string key)
    {
        try
        {
            Preferences.Remove(key);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public static AppTheme GetTheme()
    {
        // Default to System theme
        var themePreference = Preferences.Get("AppThemePreference", "System");
        // Convert string to AppTheme enum
        var savedThemeValue = themePreference switch
        {
            "System" => AppTheme.Unspecified,
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };

        return savedThemeValue;
    }

    public static bool GetRememberMe()
    {
        return Preferences.Get("RememberMe", false);
    }

    public static Guid GetLastStoredUserId()
    {
        return Guid.Parse((ReadOnlySpan<char>)Preferences.Get("LastStoredUserId", Guid.Empty.ToString()));
    }

    public static bool SetLastStoredUserId(Guid userId)
    {
        try
        {
            Preferences.Set("LastStoredUserId", userId.ToString());
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public static string GetLastStoredUsername()
    {
        return Preferences.Get("LastStoredUsername", string.Empty);
    }

    public static bool SetLastStoredUsername(string username)
    {
        try
        {
            Preferences.Set("LastStoredUsername", username);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public static bool GetTrustedDevice()
    {
        return Preferences.Get("TrustedDevice", false);
    }

    public static bool SetRememberMe(bool rememberMe)
    {
        try
        {
            Preferences.Set("RememberMe", rememberMe);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public static bool SetTrustedDevice(bool trustedDevice)
    {
        try
        {
            Preferences.Set("TrustedDevice", trustedDevice);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public static string GetLastStoredPassword()
    {
        return Preferences.Get("LastStoredPassword", string.Empty);
    }

    public static bool SetLastStoredPassword(string password)
    {
        try
        {
            Preferences.Set("LastStoredPassword", password);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public static bool GetAutoLogin()
    {
        return Preferences.Get("AutoLogin", false);
    }

    public static bool SetAutoLogin(bool autoLogin)
    {
        try
        {
            Preferences.Set("AutoLogin", autoLogin);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
}