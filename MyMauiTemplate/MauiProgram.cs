using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Mopups.Interfaces;
using Mopups.Services;
using MyMauiTemplate.Core.Services;
using MyMauiTemplate.Core.Services.Interfaces;
using MyMauiTemplate.Pages;
using MyMauiTemplate.Pages.Account;
using MyMauiTemplate.Popups.Account;

namespace MyMauiTemplate
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureMopups()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<IPopupNavigation, PopupNavigation>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<PasswordRecoveryPage>();
            builder.Services.AddTransient<PasswordResetPage>();
            builder.Services.AddTransient<ChangePasswordPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<OtpConfirmPopup>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
