using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text.Json;
using SmartRead.MVVM.Models;
using SmartRead.MVVM.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;


namespace SmartRead.MVVM.ViewModels
{
    public partial class CategoriesViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;
        public ObservableCollection<string> CategoryNames { get; set; } = new();
        public ObservableCollection<Category> Categories { get; set; } = new();

        private readonly HttpClient _httpClient;

        public CategoriesViewModel(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
            _httpClient = new HttpClient();

            // Cargar categorías en segundo plano (no esperas en el constructor)
            Task.Run(() => LoadCategoriesAsync());
        }

        public async Task LoadCategoriesAsync()
        {
            try
            {
                Console.WriteLine("PRUEBA");
                var functionKey = _configuration["AzureFunctionKey"];
                Console.WriteLine("PRUEBA1000");
                if (string.IsNullOrWhiteSpace(functionKey))
                {
                    Console.WriteLine("PRUEBAAAAAAAAAAA");
                    await Shell.Current.DisplayAlert("Error", "La AzureFunctionKey no está configurada.", "OK");
                    return;
                }
                Console.WriteLine("PRUEBA2");
                var accessToken = await _authService.GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    await Shell.Current.DisplayAlert("Error", "No se encontró el token de acceso. Inicie sesión nuevamente.", "OK");
                    return;
                }
                Console.WriteLine("PRUEBA3");
                var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}" +
          $"&action=getcategories&accesstoken={Uri.EscapeDataString(accessToken)}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    // Manejo de error
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var categoriesFromApi = JsonSerializer.Deserialize<List<Category>>(json, options);
                Console.WriteLine("Se ha llegado hasta recibir y da esto " + categoriesFromApi);

                if (categoriesFromApi != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        CategoryNames.Clear();
                        foreach (var category in categoriesFromApi)
                        {
                            Console.WriteLine($"Categoría recibida: {category.Name}");
                            Categories.Add(category);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}

