using SmartRead.MVVM.Services;

namespace SmartRead.MVVM.Views.User;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService;

    public ProfilePage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }
    private void OnCustomButtonClicked(object sender, EventArgs e)
    {
        // Desplegar el flyout (abre el menú lateral)
        Shell.Current.FlyoutIsPresented = true;
    }
    private void Button_Clicked(object sender, EventArgs e)
    {
        _authService.Logout();
        Shell.Current.GoToAsync("//login");
    }
}


             
