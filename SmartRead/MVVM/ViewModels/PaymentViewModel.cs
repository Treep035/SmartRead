using System;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using Newtonsoft.Json.Linq;

namespace SmartRead.MVVM.ViewModels
{
    public partial class PaymentViewModel : ObservableObject
    {
        private readonly IConfiguration _configuration;

        public PaymentViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [RelayCommand]
        public async Task StartPayment()
        {
            var functionKey = _configuration["AzureFunctionKey"];
            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}&action=createcheckoutsession";

            await Shell.Current.DisplayAlert("Debug", $"URL final: {url}", "OK");

            using (var httpClient = new HttpClient())
            {
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
                    var sessionId = json["sessionId"]?.ToString();

                    if (!string.IsNullOrEmpty(checkoutUrl))
                    {
                        await Launcher.Default.OpenAsync(checkoutUrl);
                        await Shell.Current.DisplayAlert("Stripe Session ID", sessionId, "OK");

                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "La URL de pago es inválida o nula.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Excepción al iniciar el pago: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        public async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync("//register");
        }
    }
}
