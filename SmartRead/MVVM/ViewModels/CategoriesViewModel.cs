using System.Collections.ObjectModel;

namespace SmartRead.MVVM.ViewModels
{
    public class CategoriesViewModel
    {
        public ObservableCollection<string> CategoryNames { get; set; }

        public CategoriesViewModel()
        {
            // Inicializamos las categorías
            CategoryNames = new ObservableCollection<string>
            {
                "Aventuras", "Fantástico", "Intriga",
                "Infantil y juvenil", "Terror", "Clásico", "Ciencia ficción",
                "Ciencia", "Humor", "Novela", "Cuentos"
            };
        }
    }
}
