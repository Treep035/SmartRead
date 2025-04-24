// SmartRead.MVVM.ViewModels/NewsViewModel.cs
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

        public IAsyncRelayCommand<Book> OpenBookCommand { get; }
        public IRelayCommand CargarPopularesCommand { get; }
        public IAsyncRelayCommand CargarRecientesCommand { get; }
        public IRelayCommand<Book> NavigateToInfoCommand { get; }

        public NewsViewModel(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;

            CargarPopularesCommand = new RelayCommand(CargarPopulares);
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

        private void CargarPopulares()
        {
            if (_isBusy) return;
            PopularSelected = true;
            NovedadesSelected = false;
            LibrosActuales.Clear();

            if (_cachedPopulares is not null)
            {
                foreach (var bk in _cachedPopulares)
                    LibrosActuales.Add(bk);
                return;
            }

            var listaHardcode = new[]
            {
                new Book { Title = "Don Quijote de la Mancha", /* …resto…*/ },
                new Book { Title = "Cien años de soledad",    /* …resto…*/ },
                new Book { Title = "El Principito",           /* …resto…*/ }
            }
            .Select(b =>
            {
                b.ParseAndSetAuthorTitleFromFilePath();
                return b;
            })
            .ToList();

            _cachedPopulares = listaHardcode;
            foreach (var bk in listaHardcode)
                LibrosActuales.Add(bk);
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

                // Nombre único para no pisar otras descargas
                string localFileName = $"{Guid.NewGuid()}.epub";
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, localFileName);

                // Descarga por streaming
                using (var client = new HttpClient())
                using (Stream httpStream = await client.GetStreamAsync(book.FileUrl))
                using (FileStream fs = File.Create(localFilePath))
                {
                    await httpStream.CopyToAsync(fs);
                }

                // Lectura del EPUB
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
    }
}
