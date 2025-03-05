using SmartRead.MVVM.Views;
using SmartRead.MVVM.Views.Book;
using SmartRead.MVVM.Views.User;

namespace SmartRead
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            
            MainPage = serviceProvider.GetRequiredService<AppShell>();
        }


    }

}
