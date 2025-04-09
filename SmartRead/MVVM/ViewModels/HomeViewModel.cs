using System;
using System.Collections.ObjectModel;
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

namespace SmartRead.MVVM.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        // Se inyectan el AuthService e IConfiguration para obtener la Azure Function Key y los tokens.
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        // Colección de categorías que se llenará con las categorías estáticas y con la respuesta de la API.
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();

        // Constructor con inyección de dependencias.
        public HomeViewModel(AuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        /// <summary>
        /// Método que obtiene las categorías invocando la API de Azure Functions
        /// y agrega además las categorías estáticas "Recomendaciones para ti" y "Seguir leyendo".
        /// </summary>
        [RelayCommand]
        public async Task LoadCategoriesAsync()
        {
            // Recuperar la Azure Function Key desde la configuración.
            var functionKey = _configuration["AzureFunctionKey"];
            if (string.IsNullOrWhiteSpace(functionKey))
            {
                await Shell.Current.DisplayAlert("Error", "La AzureFunctionKey no está configurada.", "OK");
                return;
            }

            // Recuperar el token de acceso previamente almacenado.
            var accessToken = await _authService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(accessToken))
            {
                await Shell.Current.DisplayAlert("Error", "No se encontró el token de acceso. Inicie sesión nuevamente.", "OK");
                return;
            }

            // Construir la URL para invocar la API con la acción 'getcategories' y el parámetro 'accesstoken'
            var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}" +
                      $"&action=getcategories&accesstoken={Uri.EscapeDataString(accessToken)}";

            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Primero, limpiar la colección y agregar las categorías estáticas.
                    Categories.Clear();

                    // Agregar categoría estática "Recomendaciones para ti" sin libros.
                    Categories.Add(new Category(0, "Recomendaciones para ti", new ObservableCollection<Book>()));

                    // Agregar categoría estática "Seguir leyendo" sin libros.
                    Categories.Add(new Category(0, "Seguir leyendo", new ObservableCollection<Book>()));

                    // Realizar la llamada a la API.
                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        await Shell.Current.DisplayAlert("Error", $"Error al obtener las categorías: {errorMessage}", "OK");
                        return;
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Configurar opciones de deserialización (ignorando mayúsculas/minúsculas).
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    // Deserializar la respuesta en una lista de Category (el modelo ya actualizado).
                    var categoriesFromApi = JsonSerializer.Deserialize<List<Category>>(responseContent, options);
                    if (categoriesFromApi == null)
                    {
                        await Shell.Current.DisplayAlert("Error", "La respuesta no contiene categorías válidas.", "OK");
                        return;
                    }

                    // Agregar las categorías obtenidas de la API a la colección.
                    foreach (var category in categoriesFromApi)
                    {
                        // Asegurarse de que la propiedad Books esté inicializada.
                        if (category.Books == null)
                            category.Books = new ObservableCollection<Book>();

                        Categories.Add(category);
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Excepción al obtener categorías: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        public async Task NavigateToInfo(Book book)
        {
            var info = new Info
            {
                Title = "Título de ejemplo",
                Description = "Esta es una descripción de ejemplo",
                ImageSource = book.ImageSource
            };

            await Shell.Current.Navigation.PushAsync(new InfoPage());
        }

        [RelayCommand]
        public Task NavigateToCategories()
        {
            var categoriesPopup = new CategoriesPopup();  // Crear el Popup.
            Application.Current.MainPage.ShowPopup(categoriesPopup);  // Mostrar el Popup.
            return Task.CompletedTask;
        }
    }
}
