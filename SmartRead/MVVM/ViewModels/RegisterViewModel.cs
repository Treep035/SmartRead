using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
            RegisterCommand = new RelayCommand(Register);
            NavigateToLoginCommand = new RelayCommand(NavigateToLogin);
        }

        public ICommand RegisterCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        private async void Register()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
                return;
            }

            await App.Current.MainPage.DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
            App.Current.MainPage = new LoginPage();
        }

        private async void NavigateToLogin()
        {
            App.Current.MainPage = new LoginPage();
        }
    }
}
