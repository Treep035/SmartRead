namespace SmartRead.MVVM.Views.User.Account;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}
    private void OnCustomButtonClicked(object sender, EventArgs e)
    {
        // Desplegar el flyout (abre el men� lateral)
        Shell.Current.FlyoutIsPresented = true;
    }
}