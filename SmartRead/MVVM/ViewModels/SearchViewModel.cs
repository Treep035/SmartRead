// File: SmartRead/MVVM/ViewModels/SearchViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.Models;
using SmartRead.MVVM.Services;

namespace SmartRead.MVVM.ViewModels
{
    [QueryProperty(nameof(From), "from")]
    public partial class SearchViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public IRelayCommand<Book> NavigateToInfoCommand { get; }


        public SearchViewModel(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
            _httpClient = new HttpClient();

            NavigateToInfoCommand = new RelayCommand<Book>(async book =>
            {
                if (book == null) return;
                var p = new Dictionary<string, object>
                {
                    ["book"] = book,
                    ["source"] = "search"
                };
                await Shell.Current.GoToAsync("//info", p);
            });
        }

        [ObservableProperty]
        private string from;

        [ObservableProperty]
        private string searchText;

        public ObservableCollection<Book> SearchResults { get; } = new();


        [RelayCommand]
        private async Task SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return;

            try
            {
                var functionKey = _configuration["AzureFunctionKey"]
                                  ?? throw new InvalidOperationException("AzureFunctionKey no configurada.");
                string accessToken = await _authService.GetAccessTokenAsync()
                                     ?? throw new InvalidOperationException("No se encontró token de acceso.");

                string url =
                    $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function" +
                    $"?code={functionKey}" +
                    $"&action=searchbooks" +
                    $"&query={Uri.EscapeDataString(query)}" +
                    $"&accesstoken={Uri.EscapeDataString(accessToken)}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                var books = JsonSerializer.Deserialize<Book[]>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? Array.Empty<Book>();

                SearchResults.Clear();
                foreach (var book in books)
                {
                    SearchResults.Add(book);
                }
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"[SearchViewModel] HTTP error in SearchAsync: {httpEx}");
                await Shell.Current.DisplayAlert(
                    "Error de conexión",    
                    "No se pudo conectar al servidor. Comprueba tu conexión a internet.",
                    "OK");
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine($"[SearchViewModel] JSON error in SearchAsync: {jsonEx}");
                await Shell.Current.DisplayAlert(
                    "Error de datos",
                    "Ocurrió un error al procesar los datos de los libros.",
                    "OK");
            }
            catch (InvalidOperationException invOpEx)
            {
                Debug.WriteLine($"[SearchViewModel] Config error in SearchAsync: {invOpEx}");
                await Shell.Current.DisplayAlert(
                    "Error de configuración",
                    invOpEx.Message,
                    "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SearchViewModel] Unexpected error in SearchAsync: {ex}");
                await Shell.Current.DisplayAlert(
                    "Error inesperado",
                    ex.Message,
                    "OK");
            }
        }

        [RelayCommand]
        private async Task CloseSearchAsync()
        {
            if (From == "profile")
                await Shell.Current.GoToAsync("//profile");
            else if (From == "home")
                await Shell.Current.GoToAsync("//home");
            else
                await Shell.Current.GoToAsync("//news");
        }
    }
}
