using SmartRead.MVVM.Views;

namespace SmartRead
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
