using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Models;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.Views.Book;
using System.Text;

namespace SmartRead.MVVM.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly JsonDatabaseService _jsonDatabaseService;
        private bool _isLoadingLiked;
        private bool _isLoadingMyList;
        private bool _isRemoving;
        private const int PageSize = 10;
        private const string BaseFunctionUrl = "https://functionappsmartread20250303123217.azurewebsites.net/api/Function";

        public ObservableCollection<Book> LikedBooks { get; } = new();
        public ObservableCollection<Book> MyListBooks { get; } = new();
        public ObservableCollection<Book> ContinueReadingBooks { get; } = new();

        public IRelayCommand<Book> NavigateToInfoCommand { get; }
        public IRelayCommand<Book> RemoveFromListCommand { get; }

        public ProfileViewModel(AuthService authService, IConfiguration configuration, JsonDatabaseService jsonDatabaseService)
        {
            _authService = authService;
            _configuration = configuration;
            _jsonDatabaseService = jsonDatabaseService;

            NavigateToInfoCommand = new RelayCommand<Book>(async book =>
            {
                if (book == null) return;
                var p = new Dictionary<string, object>
                {
                    ["book"] = book,
                    ["source"] = "profile"
                };
                await Shell.Current.GoToAsync("//info", p);
            });

            RemoveFromListCommand = new RelayCommand<Book>(async book => await RemoveFromListAsync(book));
        }

        [RelayCommand]
        public async Task LoadProfileAsync()
        {
            var tasks = new List<Task>();
            if (LikedBooks.Count == 0)
                tasks.Add(LoadLikedBooksAsync());
            if (MyListBooks.Count == 0)
                tasks.Add(LoadMyListBooksAsync());

            tasks.Add(LoadContinueReadingAsync());

            await Task.WhenAll(tasks);
        }

        [RelayCommand]
        public async Task LoadLikedBooksAsync()
        {
            if (_isLoadingLiked) return;

            _isLoadingLiked = true;
            try
            {
                var books = await FetchBooksFromApiAsync("getlikedbooks", 0);
                foreach (var book in books)
                    LikedBooks.Add(book);
            }
            finally { _isLoadingLiked = false; }
        }

        [RelayCommand]
        public async Task LoadMyListBooksAsync()
        {
            if (_isLoadingMyList) return;

            _isLoadingMyList = true;
            try
            {
                var books = await FetchBooksFromApiAsync("getmylist", 0);
                foreach (var book in books)
                    MyListBooks.Add(book);
            }
            finally { _isLoadingMyList = false; }
        }

        private async Task RemoveFromListAsync(Book book)
        {
            if (book == null || _isRemoving) return;
            _isRemoving = true;
            try
            {
                await RemoveBookFromApiAsync(book.IdBook);
                MyListBooks.Remove(book);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"No se pudo eliminar el libro: {ex.Message}", "OK");
            }
            finally
            {
                _isRemoving = false;
            }
        }

        private async Task RemoveBookFromApiAsync(int bookId)
        {
            var functionKey = _configuration["AzureFunctionKey"];
            if (string.IsNullOrWhiteSpace(functionKey))
                throw new InvalidOperationException("AzureFunctionKey no configurada.");

            var accessToken = await _authService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
                throw new InvalidOperationException("Token de acceso no válido.");

            var url = $"{BaseFunctionUrl}?code={functionKey}&action=removetolist&bookId={bookId}&accesstoken={Uri.EscapeDataString(accessToken)}";

            using var client = new HttpClient();
            var resp = await client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new HttpRequestException(msg);
            }
        }

        private async Task<List<Book>> FetchBooksFromApiAsync(string action, int offset)
        {
            var functionKey = _configuration["AzureFunctionKey"];
            if (string.IsNullOrWhiteSpace(functionKey))
                throw new InvalidOperationException("AzureFunctionKey no configurada.");

            var accessToken = await _authService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
                throw new InvalidOperationException("Token de acceso no válido.");

            var url = $"{BaseFunctionUrl}?code={functionKey}&action={action}" +
                      (offset > 0 ? $"&offset={offset}&limit={PageSize}" : string.Empty) +
                      $"&accesstoken={Uri.EscapeDataString(accessToken)}";

            using var client = new HttpClient();
            var resp = await client.GetAsync(url);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();
            var books = JsonSerializer.Deserialize<List<Book>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Book>();

            foreach (var bk in books)
                bk.ParseAndSetAuthorTitleFromFilePath();

            return books;
        }

        [RelayCommand]
        public async Task LoadContinueReadingAsync()
        {
            var idsToRead = await _jsonDatabaseService.GetIdBooksForRead();
            var booksToRead = await GetBooksByIdsAsync(idsToRead);

            ContinueReadingBooks.Clear();
            foreach (var book in booksToRead)
                ContinueReadingBooks.Add(book);
        }


        private async Task<List<Book>> GetBooksByIdsAsync(List<int> ids)
        {
            var functionKey = _configuration["AzureFunctionKey"];
            var accessToken = await _authService.GetAccessTokenAsync();

            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}&action=getbooksbyids&accesstoken={Uri.EscapeDataString(accessToken)}";

            using var httpClient = new HttpClient();

            // Convertir la lista de IDs a JSON
            var requestContent = new StringContent(JsonSerializer.Serialize(ids), Encoding.UTF8, "application/json");

            // Hacer la petición POST
            var response = await httpClient.PostAsync(url, requestContent);

            if (!response.IsSuccessStatusCode) return new List<Book>();

            // Leer la respuesta JSON
            var json = await response.Content.ReadAsStringAsync();

            // Deserializar los libros de la respuesta
            var books = JsonSerializer.Deserialize<List<Book>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Book>();

            // Procesar los libros si es necesario (por ejemplo, extraer autor y título desde la ruta del archivo)
            foreach (var book in books)
                book.ParseAndSetAuthorTitleFromFilePath();

            return books;
        }
    }
}
