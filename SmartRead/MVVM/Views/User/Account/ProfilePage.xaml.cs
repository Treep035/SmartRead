using SmartRead.MVVM.ViewModels;
using SmartRead.MVVM.Views.Book;
using SmartRead.ViewModels;

namespace SmartRead.MVVM.Views.User.Account;
public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = new ProfileViewModel();  // Conecta la vista con los datos
    }

    private async void OnMenuClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Menú", "Aquí irá el menú lateral", "OK");
    }

    // ✅ Método correcto para compartir
    private async void OnShareClicked(object sender, EventArgs e)
    {
        await Application.Current.MainPage.DisplayAlert("Compartir", "Funcionalidad en desarrollo", "OK");
    }
    // ✅ Método para abrir NoticiasPage cuando se haga clic en el botón "Noticias"
    private async void OnNoticiasClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("news"); // 🔹 Redirige a la página de noticias
    }
}

