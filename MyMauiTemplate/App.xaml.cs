using MyMauiTemplate.Utilities;

namespace MyMauiTemplate
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
            
            const int minimumWidth = 400;
            const int minimumHeight = 650;
            
            window.MinimumWidth = minimumWidth;
            window.MinimumHeight = minimumHeight;

            return window;

        }
    }
}
