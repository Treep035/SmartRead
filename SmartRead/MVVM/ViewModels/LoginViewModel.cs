using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartRead.MVVM.Views.User;
using SmartRead.MVVM.Views.Book;

namespace SmartRead.MVVM.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool rememberMe;

        public LoginViewModel()
        {
           
        }

        [RelayCommand]
        public async Task Login()
        {

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Debe ingresar un correo y una contraseña.", "OK");
                return;
            }

            await Shell.Current.DisplayAlert("Éxito", "Inicio de sesión exitoso", "OK");
            await Shell.Current.GoToAsync("//home");
        }

        [RelayCommand]
        public async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync("//register");
        }
    }
}
