using CommunityToolkit.Maui.Views;
using SmartRead.MVVM.ViewModels;
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
        // Verificación correcta usando la propiedad estática
        if (ProfileDropDownPopup.IsPopupOpen) return;

        try
        {
            var popup = new ProfileDropDownPopup();
            this.ShowPopup(popup);
        }
        catch (InvalidOperationException ex)
        {
            // Manejar el caso de popup ya abierto
            Console.WriteLine(ex.Message);
        }
    }

    // Método correcto para compartir
    private async void OnShareClicked(object sender, EventArgs e)
    {
        await Application.Current.MainPage.DisplayAlert("Compartir", "Funcionalidad en desarrollo", "OK");
    }
}

