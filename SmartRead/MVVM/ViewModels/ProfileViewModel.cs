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

namespace SmartRead.MVVM.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;
        private bool _isLoadingLiked;
        private bool _isLoadingMyList;
        private bool _isLoadingContinue;
        private const int PageSize = 10;

        public ObservableCollection<Book> LikedBooks { get; } = new();
        public ObservableCollection<Book> MyListBooks { get; } = new();
        public ObservableCollection<Book> ContinueReadingBooks { get; } = new();

        public IRelayCommand<Book> NavigateToInfoCommand { get; }

        public ProfileViewModel(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;

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
        }

        [RelayCommand]
        public async Task LoadProfileAsync()
        {
            var tasks = new List<Task>();
            if (LikedBooks.Count == 0)
                tasks.Add(LoadLikedBooksAsync());
            if (MyListBooks.Count == 0)
                tasks.Add(LoadMyListBooksAsync());
            //if (ContinueReadingBooks.Count == 0)
            //  tasks.Add(LoadContinueReadingAsync());

            if (tasks.Any())
                await Task.WhenAll(tasks);
        }

        [RelayCommand]
        public async Task LoadLikedBooksAsync()
        {
            if (_isLoadingLiked || LikedBooks.Count > 0)
                return;

            _isLoadingLiked = true;
            try
            {
                var books = await FetchBooksFromApiAsync("getlikedbooks", 0);
                foreach (var book in books)
                    LikedBooks.Add(book);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar libros favoritos: {ex.Message}", "OK");
            }
            finally { _isLoadingLiked = false; }
        }

        [RelayCommand]
        public async Task LoadMyListBooksAsync()
        {
            if (_isLoadingMyList || MyListBooks.Count > 0)
                return;

            _isLoadingMyList = true;
            try
            {
                var books = await FetchBooksFromApiAsync("getmylist", 0);
                foreach (var book in books)
                    MyListBooks.Add(book);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar Mi Lista: {ex.Message}", "OK");
            }
            finally { _isLoadingMyList = false; }
        }

        [RelayCommand]
        public async Task LoadContinueReadingAsync()
        {
            if (_isLoadingContinue || ContinueReadingBooks.Count > 0)
                return;

            _isLoadingContinue = true;
            try
            {
                var books = await FetchBooksFromApiAsync("getcontinuereading", 0);
                foreach (var book in books)
                    ContinueReadingBooks.Add(book);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al cargar Seguir leyendo: {ex.Message}", "OK");
            }
            finally { _isLoadingContinue = false; }
        }

        private async Task<List<Book>> FetchBooksFromApiAsync(string action, int offset)
        {
            var functionKey = _configuration["AzureFunctionKey"];
            if (string.IsNullOrWhiteSpace(functionKey))
                throw new InvalidOperationException("AzureFunctionKey no configurada.");

            var accessToken = await _authService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
                throw new InvalidOperationException("Token de acceso no válido.");

            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function" +
                      $"?code={functionKey}&action={action}" +
                      (offset > 0 ? $"&offset={offset}&limit={PageSize}" : string.Empty) +
                      $"&accesstoken={Uri.EscapeDataString(accessToken)}";

            using var client = new HttpClient();
            var resp = await client.GetAsync(url);
            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                throw new HttpRequestException(msg);
            }

            var json = await resp.Content.ReadAsStringAsync();
            var books = JsonSerializer.Deserialize<List<Book>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<Book>();

            foreach (var bk in books)
                bk.ParseAndSetAuthorTitleFromFilePath();

            return books;
        }
    }
}
