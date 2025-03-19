using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.User;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(IConfiguration configuration)
	{
		InitializeComponent();
        BindingContext = new RegisterViewModel(configuration);

    }


}