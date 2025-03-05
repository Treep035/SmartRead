using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SmartRead.MVVM.Views.User;

namespace SmartRead.MVVM.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private bool rememberMe;

        public RegisterViewModel()
        {
           
        }

        [RelayCommand]
        public async Task Register()
        {

            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await Shell.Current.DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
                return;
            }

            await Shell.Current.DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");

            await Shell.Current.GoToAsync("//login");
        }

        [RelayCommand]
        public async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}
