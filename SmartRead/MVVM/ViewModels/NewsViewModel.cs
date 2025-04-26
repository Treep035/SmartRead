using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using SmartRead.MVVM.Models;
using SmartRead.MVVM.Services;
using VersOne.Epub;

namespace SmartRead.MVVM.ViewModels
{
    public partial class NewsViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;
        private bool _isBusy;
        private List<Book>? _cachedPopulares;
        private List<Book>? _cachedRecientes;

        public ObservableCollection<Book> LibrosActuales { get; } = new ObservableCollection<Book>();

        [ObservableProperty]
        private bool popularSelected = true;

        [ObservableProperty]
        private bool novedadesSelected = false;

        [ObservableProperty]
        private Book book;

        public IAsyncRelayCommand<Book> OpenBookCommand { get; }
        public IAsyncRelayCommand CargarPopularesCommand { get; }
        public IAsyncRelayCommand CargarRecientesCommand { get; }
        public IRelayCommand<Book> NavigateToInfoCommand { get; }

        public NewsViewModel(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;

            CargarPopularesCommand = new AsyncRelayCommand(CargarPopularesAsync);
            CargarRecientesCommand = new AsyncRelayCommand(CargarRecientesAsync);
            NavigateToInfoCommand = new RelayCommand<Book>(async book =>
            {
                if (book == null) return;
                var p = new Dictionary<string, object>
                {
                    ["book"] = book,
                    ["source"] = "news"
                };
                await Shell.Current.GoToAsync("//info", p);
            });

            OpenBookCommand = new AsyncRelayCommand<Book>(OpenBookAsync);
        }

        public async Task InitializeAsync()
        {
            if (!LibrosActuales.Any())
            {
                await CargarRecientesAsync();
            }
        }

        private async Task CargarPopularesAsync()
        {
            if (_isBusy) return;
            _isBusy = true;
            try
            {
                PopularSelected = true;
                NovedadesSelected = false;
                LibrosActuales.Clear();

                if (_cachedPopulares is not null)
                {
                    foreach (var bk in _cachedPopulares)
                        LibrosActuales.Add(bk);
                    return;
                }

                var functionKey = _configuration["AzureFunctionKey"]
                                  ?? throw new InvalidOperationException("AzureFunctionKey no configurada.");

                var accessToken = await _authService.GetAccessTokenAsync()
                    ?? throw new InvalidOperationException("No se encontró token de acceso.");

                var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function" +
                          $"?code={functionKey}" +
                          $"&action=getpopularbooks" +
                          $"&accesstoken={Uri.EscapeDataString(accessToken)}";

                using var httpClient = new HttpClient();
                var resp = await httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();

                var json = await resp.Content.ReadAsStringAsync();
                var lista = JsonSerializer.Deserialize<List<Book>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<Book>();

                foreach (var bk in lista)
                    bk.ParseAndSetAuthorTitleFromFilePath();

                _cachedPopulares = lista;
                foreach (var bk in lista)
                    LibrosActuales.Add(bk);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NewsViewModel] Error en CargarPopularesAsync: {ex.Message}");
            }
            finally
            {
                _isBusy = false;
            }
        }

        private async Task CargarRecientesAsync()
        {
            if (_isBusy) return;
            _isBusy = true;

            try
            {
                PopularSelected = false;
                NovedadesSelected = true;
                LibrosActuales.Clear();

                if (_cachedRecientes is not null)
                {
                    foreach (var bk in _cachedRecientes)
                        LibrosActuales.Add(bk);
                    return;
                }

                var functionKey = _configuration["AzureFunctionKey"]
                                  ?? throw new InvalidOperationException("AzureFunctionKey no configurada.");

                var accessToken = await _authService.GetAccessTokenAsync()
                    ?? throw new InvalidOperationException("No se encontró token de acceso.");

                var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function" +
                          $"?code={functionKey}" +
                          $"&action=getrecentbooks" +
                          $"&accesstoken={Uri.EscapeDataString(accessToken)}";

                using var httpClient = new HttpClient();
                var resp = await httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();

                var json = await resp.Content.ReadAsStringAsync();
                var lista = JsonSerializer.Deserialize<List<Book>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<Book>();

                foreach (var bk in lista)
                    bk.ParseAndSetAuthorTitleFromFilePath();

                _cachedRecientes = lista;
                foreach (var bk in lista)
                    LibrosActuales.Add(bk);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NewsViewModel] Error en CargarRecientesAsync: {ex.Message}");
            }
            finally
            {
                _isBusy = false;
            }
        }

        private async Task OpenBookAsync(Book book)
        {
            if (book == null || string.IsNullOrEmpty(book.FileUrl))
            {
                await Shell.Current.DisplayAlert("Error", "URL del libro no disponible.", "OK");
                return;
            }

            try
            {
                Debug.WriteLine($"Descargando: {book.FileUrl}");

                string localFileName = $"{Guid.NewGuid()}.epub";
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, localFileName);

                using (var client = new HttpClient())
                using (Stream httpStream = await client.GetStreamAsync(book.FileUrl))
                using (FileStream fs = File.Create(localFilePath))
                {
                    await httpStream.CopyToAsync(fs);
                }

                using (FileStream epubStream = File.OpenRead(localFilePath))
                {
                    EpubBook epubBook = await Task.Run(() => EpubReader.ReadBook(epubStream));

                    var navParams = new Dictionary<string, object>
                    {
                        ["epubBook"] = epubBook
                    };
                    await Shell.Current.GoToAsync("//epub", navParams);
                }
            }
            catch (HttpRequestException)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo descargar el libro.", "OK");
            }
            catch (IOException)
            {
                await Shell.Current.DisplayAlert("Error", "Error al guardar o leer el archivo.", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error inesperado", ex.Message, "OK");
            }
        }

        [RelayCommand]
        internal async Task AddToListAsync(Book book)
        {
            if (book == null)
            {
                await Shell.Current.DisplayAlert("Error", "No se encontró el libro.", "OK");
                return;
            }

            var functionKey = _configuration["AzureFunctionKey"];
            if (string.IsNullOrWhiteSpace(functionKey))
            {
                await Shell.Current.DisplayAlert("Error", "La AzureFunctionKey no está configurada.", "OK");
                return;
            }

            var accessToken = await _authService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                await Shell.Current.DisplayAlert("Error", "No se encontró token de acceso. Inicia sesión nuevamente.", "OK");
                return;
            }

            // Construimos la URL para agregar a la lista, usando el parámetro 'book'
            string url =
                $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function" +
                $"?code={functionKey}" +
                $"&action=addtolist" +
                $"&bookId={book.IdBook}" +
                $"&accesstoken={Uri.EscapeDataString(accessToken)}";

            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Error",
                        $"Error al añadir a la lista: {errorMsg}", "OK");
                    return;
                }

                await Shell.Current.DisplayAlert("¡Listo!",
                    "El libro ha sido añadido a tu lista.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error",
                    $"Excepción al añadir a la lista: {ex.Message}", "OK");
            }
        }
    }
}
