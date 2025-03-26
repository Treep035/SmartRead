using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.User;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage(IConfiguration configuration)
    {
        InitializeComponent();
        BindingContext = new ForgotPasswordViewModel(configuration);

    }
}