using Mobile_Version.ViewModels;

namespace Mobile_Version
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}