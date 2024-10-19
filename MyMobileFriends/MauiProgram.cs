using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mopups.Hosting;
using Mopups.Interfaces;
using Mopups.Services;
using MyMobileFriends.Configuration.Constants;
using MyMobileFriends.Data.Data_Context;
using MyMobileFriends.Pages;
using MyMobileFriends.Pages.Account;
using MyMobileFriends.Popups.Account;
using MyMobileFriends.Services;
using MyMobileFriends.Services.Interfaces;

namespace MyMobileFriends
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
                    fonts.AddFont("mana.ttf", "Mana");
                });
            // builder.Services.AddDbContext<MyMauiTemplateAppContext>();
            // builder.Services.AddScoped<IUserService, UserServiceSQLite>();
            builder.Services.AddScoped<IUserService, UserServiceJson>();
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
            
            var app = builder.Build();
            
            // // Create a new scope to get the DbContext instance
            // using var scope = app.Services.CreateScope();
            // var dbContext = scope.ServiceProvider.GetRequiredService<MyMauiTemplateAppContext>();
            // // Apply any pending migrations
            // dbContext?.Database.Migrate();
            return app;
        }
    }
}
