using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.Book
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            BindingContext = new HomeViewModel();
        }
    }
}
