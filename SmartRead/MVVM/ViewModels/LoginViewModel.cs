using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartRead.MVVM.Views.User;

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
            LoginCommand = new RelayCommand(Login);
            NavigateToRegisterCommand = new RelayCommand(NavigateToRegister);
        }

        public ICommand LoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        private async void Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar un correo y una contraseña.", "OK");
                return;
            }

            await App.Current.MainPage.DisplayAlert("Éxito", "Inicio de sesión exitoso", "OK");
        }

        private async void NavigateToRegister()
        {
            App.Current.MainPage = new RegisterPage();
        }
    }
}
