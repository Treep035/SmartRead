using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.User
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(AuthService authService, IConfiguration configuration)
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(authService, configuration);
        }
    }
}
