using SmartRead.MVVM.Services;
using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.User;

public partial class LoginPage : ContentPage
{
    public LoginPage(AuthService authService)
    {
        InitializeComponent();
        BindingContext = new LoginViewModel(authService);
    }
}

