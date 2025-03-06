using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.User;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;

    }


}