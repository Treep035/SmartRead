using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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
    public partial class InfoPageViewModel : ObservableObject, IQueryAttributable
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        [ObservableProperty]
        private Book book;

        public InfoPageViewModel(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;

        }

        [RelayCommand]
        private async Task OpenBookAsync()
        {
            if (Book == null || string.IsNullOrEmpty(Book.FileUrl))
            {
                await Shell.Current.DisplayAlert("Error", "URL del libro no disponible.", "OK");
                return;
            }

            try
            {
                // Mostrar la URL en consola para debug
                Debug.WriteLine($"Enlace del libro: {Book.FileUrl}");

                using (HttpClient client = new HttpClient())
                {
                    // Se descarga el archivo EPUB
                    HttpResponseMessage response = await client.GetAsync(Book.FileUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();

                        // Se define la ruta local para guardar el archivo temporalmente
                        string localFileName = "temp.epub";
                        string localFilePath = Path.Combine(FileSystem.CacheDirectory, localFileName);

                        // Se guarda el archivo en el sistema de archivos local
                        File.WriteAllBytes(localFilePath, fileBytes);

                        // Abrir y leer el EPUB utilizando Vers-One/EpubReader
                        using (FileStream fileStream = File.OpenRead(localFilePath))
                        {
                            EpubBook epubBook = EpubReader.ReadBook(fileStream);

                            // Navegar a EpubReaderPage pasando el objeto epubBook en los parámetros
                            var navigationParams = new Dictionary<string, object>
                            {
                                { "epubBook", epubBook }
                            };
                            await Shell.Current.GoToAsync("//epub", navigationParams);
                        }
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "No se pudo descargar el libro.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("book", out var bookObj) && bookObj is Book book)
            {
                Book = book;
                Book.ParseAndSetAuthorTitleFromFilePath();
            }
        }
        [RelayCommand]
        internal async Task AddToListAsync()
        {
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

            // Construimos la URL para agregar a la lista
            string url =
                $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function" +
                $"?code={functionKey}" +
                $"&action=addtolist" +
                $"&bookId={Book.IdBook}" +
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

        internal async Task SubmitReviewAsync(int rating)
        {
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
            string url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function" +
                         $"?code={functionKey}" +
                         $"&action=addreview" +
                         $"&bookId={Book.IdBook}" +
                         $"&rating={rating}" +
                         $"&comment=" + Uri.EscapeDataString("") +
                         $"&accesstoken=" + Uri.EscapeDataString(accessToken);

            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Error",
                        $"Error al enviar valoración: {errorMsg}", "OK");
                    return;
                }

                // Si quisieras procesar un JSON de respuesta, aquí lo deserializas
                await Shell.Current.DisplayAlert("¡Gracias!", "Tu valoración ha sido enviada.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error",
                    $"Excepción al enviar valoración: {ex.Message}", "OK");
            }
        }
    }

}

