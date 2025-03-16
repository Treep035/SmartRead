using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace SmartRead.MVVM.Views.Book
{
    public partial class CategoriesPage : ContentPage
    {
        public ObservableCollection<string> CategoryNames { get; set; }

        public CategoriesPage()
        {
            InitializeComponent();
            CategoryNames = new ObservableCollection<string>
            {
                "Recomendaciones", "Aventuras", "Fantástico", "Intriga",
                "Infantil y juvenil", "Terror", "Clásico", "Ciencia ficción",
                "Ciencia", "Humor", "Novela", "Cuentos"
            };

            BindingContext = this;
        }

        private async void CloseModal(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
