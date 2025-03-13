using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.Views.Book;

namespace SmartRead.MVVM.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool rememberMe;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
        }


        [RelayCommand]
        public async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Debe ingresar un correo y una contraseña.", "OK");
                return;
            }
            _authService.Login();
            await Shell.Current.GoToAsync($"//home");
            // Aquí se simula el login sin consultar ningún servicio:
            //App.IsLoggedIn = true; // Se marca que el usuario inició sesión

            await Shell.Current.DisplayAlert("Éxito", "Inicio de sesión exitoso", "OK");
        }

        [RelayCommand]
        public async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync("//register");
        }

        [RelayCommand]
        public async Task NavigateToForgotPassword()
        {
            await Shell.Current.GoToAsync("//forgotpassword");
        }
    }
}
