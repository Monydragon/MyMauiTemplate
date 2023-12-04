using System.Text.RegularExpressions;

namespace MyMauiTemplate.Core.RegexExpressions;

public static class RegexExpressions
{
    /// <summary>
    ///  Regex for password: Minimum eight characters, at least one uppercase letter, one lowercase letter, one number and one special character
    /// </summary>
    public static readonly Regex PasswordRegex = new Regex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,}$");
}