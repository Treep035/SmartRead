using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Models;
using SmartRead.MVVM.Services;

namespace SmartRead.MVVM.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;  

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool rememberMe;

        // Se inyectan AuthService e IConfiguration en el constructor
        public LoginViewModel(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [RelayCommand]
        public async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Debe ingresar un correo y una contraseña.", "OK");
                return;
            }

            if (RememberMe)
            {
                bool success = await LoginAsync(Email, Password);
                if (!success)
                {
                    // Se detiene el proceso si no se pudo iniciar sesión correctamente.
                    return;
                }
            }
            else
            {
                bool success = await LoginAsync(Email, Password);
                if (!success)
                {
                    // Se detiene el proceso si no se pudo iniciar sesión correctamente.
                    return;
                }
            }

            _authService.Login();
            await Shell.Current.GoToAsync($"//home");
            await Shell.Current.DisplayAlert("Éxito", "Inicio de sesión exitoso", "OK");
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            // Obtener la clave de la Azure Function desde appsettings.json mediante IConfiguration
            var functionKey = _configuration["AzureFunctionKey"];
            await Shell.Current.DisplayAlert("Debug", $"Valor de functionKey: {functionKey}", "OK");

            // Construir la URL con los parámetros utilizando la clave obtenida y la acción "login"
            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}&action=login&username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(password)}";

            // Mostrar la URL final para depuración
            await Shell.Current.DisplayAlert("Debug", $"URL final: {url}", "OK");

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        await Shell.Current.DisplayAlert("Error", $"Error al iniciar sesión: {errorMessage}", "OK");
                        return false;
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Usar opciones para ignorar mayúsculas/minúsculas en los nombres de las propiedades
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, options);

                    if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                    {
                        await Shell.Current.DisplayAlert("Error", "La respuesta no contiene tokens válidos.", "OK");
                        return false;
                    }

                    // Almacenar los tokens de forma segura
                    await _authService.SaveAccessTokenAsync(tokenResponse.AccessToken);
                    await _authService.SaveRefreshTokenAsync(tokenResponse.RefreshToken);

                    // Mostrar los tokens en un DisplayAlert
                    await Shell.Current.DisplayAlert("Tokens",
                        $"Access Token:\n{tokenResponse.AccessToken}\n\nRefresh Token:\n{tokenResponse.RefreshToken}", "OK");

                    return true;
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
                    return false;
                }
            }
        }


        // public async Task LoginAsync(string username, string password)
        // {
        //     // Lógica para autenticar al usuario (simulación).
        //     // string accessToken = _authService.GenerateRandomToken(32);
        //     // string refreshToken = _authService.GenerateRandomToken(64);

        //     // await _authService.SaveAccessTokenAsync(accessToken);
        //     // await _authService.SaveRefreshTokenAsync(refreshToken);
        //     // Console.WriteLine($"Access Token: {accessToken}");
        //     // Console.WriteLine($"Refresh Token: {refreshToken}");
        // }

        // public async Task GetStoredTokensAsync()
        // {
        //     // var accessToken = await _authService.GetAccessTokenAsync();
        //     // var refreshToken = await _authService.GetRefreshTokenAsync();

        //     // Console.WriteLine($"Access Token: {accessToken}");
        //     // Console.WriteLine($"Refresh Token: {refreshToken}");
        // }



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
