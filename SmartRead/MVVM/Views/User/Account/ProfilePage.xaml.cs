using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.User.Account;
public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService;
    public ProfilePage(AuthService authService, IConfiguration configuration, JsonDatabaseService jsonDatabaseService)
    {
        InitializeComponent();
        _authService = authService;
        BindingContext = new ProfileViewModel(authService, configuration, jsonDatabaseService); 
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
            var popup = new ProfileDropDownPopup(_authService);
            this.ShowPopup(popup);
        }
        catch (InvalidOperationException ex)
        {
            // Manejar el caso de popup ya abierto
            Console.WriteLine(ex.Message);
        }
    }
}

