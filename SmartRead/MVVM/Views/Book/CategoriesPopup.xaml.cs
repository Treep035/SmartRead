using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Views;
using SmartRead.MVVM.ViewModels;
using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.Models;

namespace SmartRead.MVVM.Views.Book
{
    public partial class CategoriesPopup : Popup
    {
        public CategoriesPopup(AuthService authService, IConfiguration configuration)
        {
            InitializeComponent();
            BindingContext = new CategoriesViewModel(authService, configuration);
        }

        // Método que cierra el popup cuando el botón es presionado
        private void ClosePopup(object sender, EventArgs e)
        {
            this.Close(); 
        }

        private async void OnCategorySelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0)
                return;

            var selectedCategory = e.CurrentSelection[0] as Category;
            if (selectedCategory != null)
            {
                // Cerrar el popup
                this.Close();

                // Navegar a la página de inicio y pasar el parámetro
                await Shell.Current.GoToAsync($"//home?selectedCategory={Uri.EscapeDataString(selectedCategory.Name)}");
            }
        }
    }
}
