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
    }
}