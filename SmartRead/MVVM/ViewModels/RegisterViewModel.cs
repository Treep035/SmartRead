using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;

namespace SmartRead.MVVM.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IConfiguration _configuration;

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

        public RegisterViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
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

            bool success = await RegisterAsync(Username, Email, Password);
            if (success)
            {
                await Shell.Current.DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
                await Shell.Current.GoToAsync("//login");
            }
        }

        private async Task<bool> RegisterAsync(string username, string email, string password)
        {
            // Obtener la clave de la Azure Function desde appsettings.json mediante IConfiguration
            var functionKey = _configuration["AzureFunctionKey"];

            // Construir la URL con la acción "register"
            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}&action=register&username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(password)}&email={Uri.EscapeDataString(email)}";

            await Shell.Current.DisplayAlert("Debug", $"URL final: {url}", "OK");

            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Se utiliza POST para el registro
                    var response = await httpClient.PostAsync(url, null);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        await Shell.Current.DisplayAlert("Error", $"Error al registrar usuario: {errorMessage}", "OK");
                        return false;
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
                    return false;
                }
            }
        }


        [RelayCommand]
        public async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}
