using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
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
        private readonly JsonDatabaseService _jsonDatabaseService;
        private readonly Dictionary<int, bool> _isLoadingBooks = new();
        private bool _isCategoryPopupOpen = false;
        private bool _selectedCategory = false;
        private string _selectedCategoryLabel = "Categorías";
        private string _selectedCategoryImage = "down";
        private const int PageSize = 10;
        private readonly Dictionary<int, bool> _noMoreBooks = new();
        private List<Category> _originalCategories = new List<Category>();

        public string SelectedCategoryImageWithExtension => $"{_selectedCategoryImage}.png";

        public bool SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                    UpdateCategories();
            }
        }

        public string SelectedCategoryLabel
        {
            get => _selectedCategoryLabel;
            set
            {
                if (SetProperty(ref _selectedCategoryLabel, value) && _selectedCategory)
                    UpdateCategories();
            }
        }

        public string SelectedCategoryImage
        {
            get => _selectedCategoryImage;
            set
            {
                SetProperty(ref _selectedCategoryImage, value);
                OnPropertyChanged(nameof(SelectedCategoryImageWithExtension));
            }
        }

        public ObservableCollection<Category> Categories { get; } = new();
        public ObservableCollection<Category> FilteredCategories { get; } = new();

        /// <summary>
        /// Acceso público de la lista original para inyectarla en el popup.
        /// </summary>
        public IReadOnlyList<Category> OriginalCategories => _originalCategories;

        public HomeViewModel(AuthService authService, IConfiguration configuration, JsonDatabaseService jsonDatabaseService)
        {
            _authService = authService;
            _configuration = configuration;
            _jsonDatabaseService = jsonDatabaseService;
        }

        private void UpdateCategories()
        {
            if (SelectedCategory)
            {
                FilteredCategories.Clear();
                var match = _originalCategories
                    .FirstOrDefault(c => c.Name.Equals(SelectedCategoryLabel, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                    FilteredCategories.Add(match);
            }
            else
            {
                FilteredCategories.Clear();
            }
        }

        [RelayCommand]
        public async Task LoadCategoriesAsync()
        {
            if (Categories.Count == 0)
            {
                // 1) Cargar IDs de libros y validar
                var topBookIds = await _jsonDatabaseService.LoadBooksAndReadingTimeAsync();
                if (topBookIds == null || topBookIds.Count == 0)
                {
                    await Shell.Current.DisplayAlert("Debug IDs", "No hay IDs para enviar.", "OK");
                    return;
                }

                // 2) Clave y token
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

                // 3) Construir URL GET con bookIds en la query
                var idsParam = Uri.EscapeDataString(string.Join(",", topBookIds));
                var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function" +
                          $"?code={functionKey}" +
                          $"&action=getrecommendedbooksbyids" +
                          $"&bookIds={idsParam}" +
                          $"&accesstoken={Uri.EscapeDataString(accessToken)}";

                using var http = new HttpClient();
                var response = await http.GetAsync(url);

                // 4) Mostrar en la app URL, status y JSON para depuración
                var raw = await response.Content.ReadAsStringAsync();
                await Shell.Current.DisplayAlert(
                    "Debug getrecommendedbooksbyids",
                    $"URL: {url}\nStatus: {(int)response.StatusCode} {response.StatusCode}\n\n{raw}",
                    "OK"
                );

                if (!response.IsSuccessStatusCode)
                    return;

                // 5) Deserializar y procesar
                var recommendedBooks = JsonSerializer.Deserialize<List<Book>>(raw, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<Book>();

                foreach (var book in recommendedBooks)
                {
                    try
                    {
                        book.ParseAndSetAuthorTitleFromFilePath();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error al procesar libro: " + ex.Message);
                    }
                }

                var booksToDisplay = recommendedBooks.Take(10).ToList();
                Categories.Add(new Category(0, "Recomendaciones para ti", new ObservableCollection<Book>(booksToDisplay)));
            }

            // ======== Resto de LoadCategoriesAsync sin cambios ========

            // Actualizar siempre "Seguir leyendo"
            var existing = Categories.FirstOrDefault(c => c.Name == "Seguir leyendo");
            if (existing != null)
                Categories.Remove(existing);

            var idsToRead = await _jsonDatabaseService.GetIdBooksForRead();
            var booksToRead = await GetBooksByIdsAsync(idsToRead);
            if (booksToRead.Count > 0)
            {
                Categories.Insert(1, new Category(0, "Seguir leyendo", new ObservableCollection<Book>(booksToRead)));
            }

            var functionKey2 = _configuration["AzureFunctionKey"];
            if (string.IsNullOrWhiteSpace(functionKey2))
            {
                await Shell.Current.DisplayAlert("Error", "La AzureFunctionKey no está configurada.", "OK");
                return;
            }

            var accessToken2 = await _authService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken2))
            {
                await Shell.Current.DisplayAlert("Error", "No se encontró el token de acceso. Inicie sesión nuevamente.", "OK");
                return;
            }

            var url2 = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function" +
                       $"?code={functionKey2}" +
                       $"&action=getcategories" +
                       $"&accesstoken={Uri.EscapeDataString(accessToken2)}";

            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(url2);
                if (!response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Error", $"Error al obtener categorías: {msg}", "OK");
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<Category>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (list == null) return;

                var toAdd = new List<Category>();
                foreach (var cat in list)
                {
                    if (!Categories.Any(c => c.IdCategory == cat.IdCategory))
                    {
                        cat.Books ??= new ObservableCollection<Book>();
                        toAdd.Add(cat);
                        _noMoreBooks[cat.IdCategory] = false;
                    }
                }
                foreach (var cat in toAdd)
                    Categories.Add(cat);

                _originalCategories = Categories.ToList();

                var loadSnapshot = Categories.Where(c => c.IdCategory != 0).ToList();
                foreach (var cat in loadSnapshot)
                {
                    if (cat.Books.Count == 0)
                        await LoadMoreBooksByCategoryAsync(cat);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Excepción al obtener categorías: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task LoadMoreBooksByCategoryAsync(Category category)
        {
            if (_isLoadingBooks.TryGetValue(category.IdCategory, out var running) && running)
                return;

            _isLoadingBooks[category.IdCategory] = true;
            try
            {
                var functionKey = _configuration["AzureFunctionKey"];
                var accessToken = await _authService.GetAccessTokenAsync();
                var offset = category.Books.Count;
                var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}" +
                          $"&action=getbooksbycategory&categoryId={category.IdCategory}&offset={offset}&limit={PageSize}" +
                          $"&accesstoken={Uri.EscapeDataString(accessToken)}";

                using var httpClient = new HttpClient();
                var resp = await httpClient.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                {
                    var msg = await resp.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Error", $"Error al obtener libros: {msg}", "OK");
                    return;
                }

                var json = await resp.Content.ReadAsStringAsync();
                var books = JsonSerializer.Deserialize<List<Book>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (books == null || books.Count == 0)
                {
                    _noMoreBooks[category.IdCategory] = true;
                    return;
                }

                foreach (var bk in books)
                {
                    bk.ParseAndSetAuthorTitleFromFilePath();
                    category.Books.Add(bk);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Excepción al obtener libros: {ex.Message}", "OK");
            }
            finally
            {
                _isLoadingBooks[category.IdCategory] = false;
            }
        }

        private async Task<List<Book>> GetBooksByIdsAsync(List<int> ids)
        {
            var functionKey = _configuration["AzureFunctionKey"];
            var accessToken = await _authService.GetAccessTokenAsync();

            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}&action=getbooksbyids&accesstoken={Uri.EscapeDataString(accessToken)}";

            using var httpClient = new HttpClient();
            var requestContent = new StringContent(JsonSerializer.Serialize(ids), System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, requestContent);

            if (!response.IsSuccessStatusCode) return new List<Book>();

            var json = await response.Content.ReadAsStringAsync();
            var books = JsonSerializer.Deserialize<List<Book>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Book>();

            foreach (var book in books)
                book.ParseAndSetAuthorTitleFromFilePath();

            ids.Reverse();
            var booksById = books.ToDictionary(b => b.IdBook);
            return ids.Where(id => booksById.ContainsKey(id)).Select(id => booksById[id]).ToList();
        }

        [RelayCommand]
        public async Task NavigateToInfo(Book book)
        {
            var p = new Dictionary<string, object> { ["book"] = book };
            await Shell.Current.GoToAsync("//info", p);
        }

        [RelayCommand]
        public Task NavigateToCategories()
        {
            if (SelectedCategory)
            {
                SelectedCategory = false;
                SelectedCategoryLabel = "Categorías";
                SelectedCategoryImage = "down";
            }
            else
            {
                if (_isCategoryPopupOpen)
                    return Task.CompletedTask;

                var popup = new CategoriesPopup(_authService, _configuration, OriginalCategories);
                _isCategoryPopupOpen = true;
                popup.Closed += (_, __) => { _isCategoryPopupOpen = false; };
                Application.Current.MainPage.ShowPopup(popup);
            }

            return Task.CompletedTask;
        }

        [RelayCommand]
        public async Task NavigateToSearch()
        {
            var p = new Dictionary<string, object>
            {
                ["from"] = "home"
            };
            await Shell.Current.GoToAsync("///search", p);
        }
    }
}
