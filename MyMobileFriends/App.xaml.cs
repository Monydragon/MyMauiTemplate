using MyMobileFriends.Configuration.Constants;
using MyMobileFriends.Utilities;

namespace MyMobileFriends
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            UserAppTheme = PreferencesHelper.GetTheme();
            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
            
            window.MinimumWidth = AppConstants.WindowMinimumWidth;
            window.MinimumHeight = AppConstants.WindowMinimumHeight;

            return window;

        }
    }
}
