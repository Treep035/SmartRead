using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using Newtonsoft.Json.Linq;

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

        [ObservableProperty]
        private string sessionId;

        public RegisterViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [RelayCommand]
        public async Task StartPayment()
        {
            var functionKey = _configuration["AzureFunctionKey"];
            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}&action=createcheckoutsession";

            await Shell.Current.DisplayAlert("Debug", $"URL final: {url}", "OK");

            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Error", $"Error en la solicitud de pago: {errorContent}", "OK");
                    return;
                }

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                var checkoutUrl = json["url"]?.ToString();
                var sessionIdValue = json["sessionId"]?.ToString();

                if (!string.IsNullOrEmpty(checkoutUrl) && !string.IsNullOrEmpty(sessionIdValue))
                {
                    SessionId = sessionIdValue;
                    await Launcher.Default.OpenAsync(checkoutUrl);
                    //await Shell.Current.DisplayAlert("Stripe Session ID", sessionIdValue, "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No se obtuvo una URL o sessionId válidos.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Excepción al iniciar el pago: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task Register()
        {
            // Validaciones de campos
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

            // Si no hay sessionId, iniciamos el pago primero
            if (string.IsNullOrEmpty(SessionId))
            {
                await StartPayment();

                // Tras intentar StartPayment, comprobamos de nuevo
                if (string.IsNullOrEmpty(SessionId))
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo obtener sessionId. El registro requiere pago previo.", "OK");
                    return;
                }
            }

            // Llamada al registro con sessionId válido
            bool success = await RegisterAsync(Username, Email, Password, SessionId);
            if (success)
            {
                await Shell.Current.DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
                await Shell.Current.GoToAsync("//login");
            }
        }

        private async Task<bool> RegisterAsync(string username, string email, string password, string sessionId)
        {
            var functionKey = _configuration["AzureFunctionKey"];
            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function" +
                      $"?code={functionKey}" +
                      $"&action=register" +
                      $"&username={Uri.EscapeDataString(username)}" +
                      $"&password={Uri.EscapeDataString(password)}" +
                      $"&email={Uri.EscapeDataString(email)}" +
                      $"&sessionId={Uri.EscapeDataString(sessionId)}";


            using var httpClient = new HttpClient();
            try
            {
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

        [RelayCommand]
        public async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("//login");
        }

        [RelayCommand]
        public async Task NavigateToPayment()
        {
            await Shell.Current.GoToAsync("//payment");
        }
    }
}