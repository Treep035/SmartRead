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

        private readonly Dictionary<int, bool> _isLoadingBooks = new();

        private bool _selectedCategory = false;
        private string _selectedCategoryLabel = "Categorias";
        private string _selectedCategoryImage = "down";

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

        private const int PageSize = 10;
        private readonly Dictionary<int, bool> _noMoreBooks = new();

        public ObservableCollection<Category> Categories { get; } = new();

        public HomeViewModel(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        private void UpdateCategories()
        {
            // Guardamos una copia antes de modificar
            List<Category> snapshot;
            if (_selectedCategory)
            {
                // solo la que coincide
                snapshot = Categories
                    .Where(c => c.Name.Equals(_selectedCategoryLabel, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                // restauramos el orden original
                snapshot = _originalCategories.ToList();
            }

            Categories.Clear();
            foreach (var cat in snapshot)
                Categories.Add(cat);
        }

        [RelayCommand]
        public async Task LoadCategoriesAsync()
        {
            if (Categories.Count > 1)
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

            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}" +
                      $"&action=getcategories&accesstoken={Uri.EscapeDataString(accessToken)}";

            using var httpClient = new HttpClient();
            try
            {
                if (Categories.Count == 0)
                {
                    Categories.Add(new Category(0, "Recomendaciones para ti", new ObservableCollection<Book>()));
                    Categories.Add(new Category(0, "Seguir leyendo", new ObservableCollection<Book>()));
                }

                var response = await httpClient.GetAsync(url);
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

                // lista temporal para no tocar Categories en el foreach
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

                // snapshot para iterar sin que rompa si UpdateCategories limpia
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
                var popup = new CategoriesPopup(_authService, _configuration);
                Application.Current.MainPage.ShowPopup(popup);
            }
            return Task.CompletedTask;
        }

        [RelayCommand]
        public async Task NavigateToSearch()
            => await Shell.Current.GoToAsync("//search");
    }
}
