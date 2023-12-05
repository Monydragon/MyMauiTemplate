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
    }
}
