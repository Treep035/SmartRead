using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.User.Account;
public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    private async void OnMenuClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Menú", "Aquí irá el menú lateral", "OK");
    }

    // ✅ Método correcto para compartir
    private async void OnShareClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Compartir", "Aquí puedes compartir este libro", "OK");
    }
}
