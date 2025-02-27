namespace SmartRead.MVVM.Views.User;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}
    private void OnRegisterTapped(object sender, TappedEventArgs e)
    {
        App.Current.MainPage = new RegisterPage();
    }
}