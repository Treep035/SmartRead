using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using SmartRead.MVVM.Models;
using VersOne.Epub; // Asegúrate de tener instalada la librería Vers-One/EpubReader desde NuGet

namespace SmartRead.MVVM.ViewModels
{
    public partial class InfoPageViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private Book book;

        // Comando para abrir y visualizar el libro utilizando Vers-One/EpubReader
        public IAsyncRelayCommand OpenBookCommand { get; }

        public InfoPageViewModel()
        {
            OpenBookCommand = new AsyncRelayCommand(OpenBookAsync);
        }

        /// <summary>
        /// Descarga el libro desde la URL, lo guarda localmente, lo lee utilizando Vers-One/EpubReader
        /// y navega a EpubReaderPage pasando el objeto EpubBook.
        /// </summary>
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
    }
}
