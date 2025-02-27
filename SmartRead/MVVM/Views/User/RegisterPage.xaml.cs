namespace SmartRead.MVVM.Views.User;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}

    private void OnLoginTapped(object sender, TappedEventArgs e)
    {
        App.Current.MainPage = new LoginPage();
    }
}