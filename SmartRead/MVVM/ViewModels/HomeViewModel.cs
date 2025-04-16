using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Models;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.Views.Book;
using System.Diagnostics;

namespace SmartRead.MVVM.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        // Tamaño de página para la carga paginada.
        private const int PageSize = 10;

        // Diccionario para llevar un seguimiento si ya se cargaron todos los libros de una categoría.
        private readonly Dictionary<int, bool> _noMoreBooks = new Dictionary<int, bool>();

        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();

        public HomeViewModel(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [RelayCommand]
        public async Task LoadCategoriesAsync()
        {
            // Si ya se han cargado las categorías, no las volvemos a cargar.
            if (Categories != null && Categories.Count > 0)
            {
                Debug.WriteLine("Las categorías ya están cargadas; se omite la carga.");
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
                await Shell.Current.DisplayAlert("Error", "No se encontró el token de acceso. Inicie sesión nuevamente.", "OK");
                return;
            }

            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}" +
                      $"&action=getcategories&accesstoken={Uri.EscapeDataString(accessToken)}";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Si queremos conservar los datos previos, no se hace Categories.Clear()
                    // Categories.Clear();

                    // Agregar categorías estáticas si aún no están en la colección.
                    if (Categories.Count == 0)
                    {
                        Categories.Add(new Category(0, "Recomendaciones para ti", new ObservableCollection<Book>()));
                        Categories.Add(new Category(0, "Seguir leyendo", new ObservableCollection<Book>()));
                    }

                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        await Shell.Current.DisplayAlert("Error", $"Error al obtener las categorías: {errorMessage}", "OK");
                        return;
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    var categoriesFromApi = JsonSerializer.Deserialize<List<Category>>(responseContent, options);
                    if (categoriesFromApi == null)
                    {
                        await Shell.Current.DisplayAlert("Error", "La respuesta no contiene categorías válidas.", "OK");
                        return;
                    }

                    // Agregar las categorías obtenidas de la API sin eliminar las que ya están cargadas.
                    foreach (var category in categoriesFromApi)
                    {
                        // Evitar duplicados (si el IdCategory ya existe)
                        if (!Categories.Any(c => c.IdCategory == category.IdCategory))
                        {
                            if (category.Books == null)
                                category.Books = new ObservableCollection<Book>();

                            Categories.Add(category);
                            // Inicialmente se asume que la categoría tiene más libros.
                            _noMoreBooks[category.IdCategory] = false;
                        }
                    }

                    // Para cada categoría obtenida (excluyendo las estáticas con IdCategory 0) cargamos la primera tanda de libros,
                    // siempre y cuando aún no se hayan cargado.
                    foreach (var category in Categories.Where(c => c.IdCategory != 0))
                    {
                        // Si la colección de libros está vacía, se realiza la carga inicial.
                        if (category.Books == null || category.Books.Count == 0)
                        {
                            await LoadMoreBooksByCategoryAsync(category);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Excepción al obtener categorías: {ex.Message}", "OK");
                }
            }
        }


        /// <summary>
        /// Carga inicial o incremental de libros para una categoría.
        /// Si se solicita carga incremental, no se limpia la colección existente.
        /// </summary>
        [RelayCommand]
        public async Task LoadMoreBooksByCategoryAsync(Category category)
        {
            if (category == null)
            {
                await Shell.Current.DisplayAlert("Error", "No se seleccionó ninguna categoría.", "OK");
                return;
            }

            // Si ya se determinó que no hay más libros para esa categoría, se sale.
            if (_noMoreBooks.TryGetValue(category.IdCategory, out bool noMore) && noMore)
                return;

            var functionKey = _configuration["AzureFunctionKey"];
            if (string.IsNullOrWhiteSpace(functionKey))
            {
                await Shell.Current.DisplayAlert("Error", "La AzureFunctionKey no está configurada.", "OK");
                return;
            }

            var accessToken = await _authService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                await Shell.Current.DisplayAlert("Error", "No se encontró el token de acceso. Inicie sesión nuevamente.", "OK");
                return;
            }

            // Offset = cantidad de libros ya cargados, limit = PageSize.
            int currentOffset = category.Books.Count;

            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}" +
                      $"&action=getbooksbycategory&categoryId={category.IdCategory}" +
                      $"&offset={currentOffset}&limit={PageSize}" +
                      $"&accesstoken={Uri.EscapeDataString(accessToken)}";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        await Shell.Current.DisplayAlert("Error", $"Error al obtener los libros: {errorMessage}", "OK");
                        return;
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    var booksFromApi = JsonSerializer.Deserialize<List<Book>>(responseContent, options);
                    if (booksFromApi == null || booksFromApi.Count == 0)
                    {
                        // Marcar que ya no hay más libros para esta categoría.
                        _noMoreBooks[category.IdCategory] = true;
                        return;
                    }

                    foreach (var book in booksFromApi)
                    {
                        book.ParseAndSetAuthorTitleFromFilePath();
                        Debug.WriteLine($"[LoadMoreBooksByCategoryAsync] CoverImageUrl para '{book.Title}': {book.CoverImageUrl}");
                        category.Books.Add(book);
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Excepción al obtener libros: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        public async Task NavigateToInfo(Book book)
        {
            try
            {
                // Navegar a la página InfoPage, pasando el objeto Book como parámetro.
                var navigationParameters = new Dictionary<string, object>
                {
                    { "book", book }
                };

                await Shell.Current.GoToAsync($"//info", navigationParameters);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo navegar a la información del libro: {ex.Message}", "OK");
            }
        }


        [RelayCommand]
        public Task NavigateToCategories()
        {
            var categoriesPopup = new CategoriesPopup();
            Application.Current.MainPage.ShowPopup(categoriesPopup);
            return Task.CompletedTask;
        }
    }
}
