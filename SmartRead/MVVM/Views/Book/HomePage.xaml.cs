using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.Book
{
    public partial class HomePage : ContentPage
    {
        public HomePage(AuthService authService, IConfiguration configuration)
        {
            InitializeComponent();
            BindingContext = new HomeViewModel(authService, configuration);
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Asegurarse de que el BindingContext sea del tipo HomeViewModel
            if (BindingContext is HomeViewModel viewModel)
            {
                await viewModel.LoadCategoriesAsync();
            }
        }
    }
}
