using SmartRead.MVVM.ViewModels; // Importar el ViewModel
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Models; // Para el tipo Info

namespace SmartRead.MVVM.Views.Book
{
    public partial class InfoPage : ContentPage
    {
        public InfoPage()
        {
            InitializeComponent();
            BindingContext = new InfoPageViewModel(); // Establecer el ViewModel como BindingContext
        }
        private async void ClosePopup(object sender, EventArgs e)
        {
            // Si la página se abrió de forma modal:
            // await Navigation.PopModalAsync();

            // Si la página se ha navegado usando Navigation.PushAsync, usa:
            await Shell.Current.GoToAsync("..");
        }
    }
}