using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartRead.MVVM.Views.User;
using SmartRead.MVVM.Views.Book;

namespace SmartRead.MVVM.ViewModels
{
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string code;

        public ForgotPasswordViewModel()
        {
           
        }

        [RelayCommand]
        public async Task SendCodeToEmail()
        {

            if (string.IsNullOrWhiteSpace(Email))
            {
                await Shell.Current.DisplayAlert("Error", "Debe ingresar un correo para enviar el código.", "OK");
                return;
            }

            await Shell.Current.DisplayAlert("Éxito", "Código enviado exitosamente", "OK");
        }

        [RelayCommand]
        public async Task SendCode()
        {

            if (string.IsNullOrWhiteSpace(Code))
            {
                await Shell.Current.DisplayAlert("Error", "Debe ingresar un código.", "OK");
                return;
            }

            await Shell.Current.DisplayAlert("Éxito", "Código correcto", "OK");
            //await Shell.Current.GoToAsync("//changepassword");
        }

        [RelayCommand]
        public async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}
