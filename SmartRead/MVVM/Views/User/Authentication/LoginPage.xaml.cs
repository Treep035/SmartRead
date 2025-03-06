using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.User;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

}