namespace SmartRead.MVVM.Views.User;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
	}
    private void OnCustomButtonClicked(object sender, EventArgs e)
    {
        // Desplegar el flyout (abre el men� lateral)
        Shell.Current.FlyoutIsPresented = true;
    }
}