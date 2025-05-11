using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.User.Account;
public partial class ProfilePage : ContentPage
{
    public ProfilePage(AuthService authService, IConfiguration configuration)
    {
        InitializeComponent();
        BindingContext = new ProfileViewModel(authService, configuration); 
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProfileViewModel viewModel)
        {
            await viewModel.LoadProfileAsync();
        }
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
}

