using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartRead.MVVM.Views.User;
using SmartRead.MVVM.Views.Book;
using SmartRead.MVVM.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using System.Text.Json;
using System.Text;

namespace SmartRead.MVVM.ViewModels
{
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        private readonly IConfiguration _configuration;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string userCode;

        public ForgotPasswordViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [RelayCommand]
        public async Task SendCodeToEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await Shell.Current.DisplayAlert("Error", "Debe ingresar un correo para enviar el código.", "OK");
                return;
            }

            bool result = await SendCodeAsync(Email);
            if (result)
            {
                await Shell.Current.DisplayAlert("Éxito", "El código de recuperación ha sido enviado a tu correo.", "OK");
            }
            // En caso de error, SendCodeAsync ya muestra el mensaje correspondiente.
        }

        [RelayCommand]
        public async Task VerifyCode()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await Shell.Current.DisplayAlert("Error", "Debe ingresar el correo.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(UserCode))
            {
                await Shell.Current.DisplayAlert("Error", "Debe ingresar el código.", "OK");
                return;
            }

            bool result = await VerifyCodeAsync(Email, UserCode);
            if (result)
            {
                await Shell.Current.DisplayAlert("Éxito", "Código correcto. ¡Puedes cambiar tu contraseña ahora!", "OK");
                // Aquí podrías navegar a la pantalla de cambio de contraseña, por ejemplo:
                // await Shell.Current.GoToAsync("//ChangePasswordPage");
            }
            // En caso de error, VerifyCodeAsync ya muestra el mensaje correspondiente.
        }

        [RelayCommand]
        public async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("//login");
        }

        // Método auxiliar para enviar el código de recuperación mediante la API (acción sendcode)
        private async Task<bool> SendCodeAsync(string email)
        {
            var functionKey = _configuration["AzureFunctionKey"];
            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}&action=sendcode&email={Uri.EscapeDataString(email)}";

            await Shell.Current.DisplayAlert("Debug", $"URL final: {url}", "OK");

            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Se utiliza POST para enviar la solicitud, sin body adicional
                    var response = await httpClient.PostAsync(url, null);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        await Shell.Current.DisplayAlert("Error", $"Error al enviar el código: {errorMessage}", "OK");
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

        // Método auxiliar para verificar el código mediante la API (acción validatecode)
        private async Task<bool> VerifyCodeAsync(string email, string code)
        {
            var functionKey = _configuration["AzureFunctionKey"];
            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}&action=validatecode&email={Uri.EscapeDataString(email)}&resetcode={Uri.EscapeDataString(code)}";

            await Shell.Current.DisplayAlert("Debug", $"URL final: {url}", "OK");

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        await Shell.Current.DisplayAlert("Error", $"Error al verificar el código: {errorMessage}", "OK");
                        return false;
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    // Se utiliza la misma clase RecoveryResponse, la cual debe tener las propiedades Code e IsValid.
                    var recoveryResponse = JsonSerializer.Deserialize<RecoveryResponse>(responseBody, options);
                    if (recoveryResponse != null && recoveryResponse.IsValid)
                    {
                        return true;
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Código incorrecto o expirado.", "OK");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
                    return false;
                }
            }
        }
    }
}
