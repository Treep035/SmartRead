using System.Collections.ObjectModel;

namespace SmartRead.MVVM.ViewModels
{
    public class CategoriesViewModel
    {
        public ObservableCollection<string> CategoryNames { get; set; }

        public CategoriesViewModel()
        {
            // Inicializamos las categor�as
            CategoryNames = new ObservableCollection<string>
            {
                "Aventuras", "Fant�stico", "Intriga",
                "Infantil y juvenil", "Terror", "Cl�sico", "Ciencia ficci�n",
                "Ciencia", "Humor", "Novela", "Cuentos"
            };
        }
    }
}
