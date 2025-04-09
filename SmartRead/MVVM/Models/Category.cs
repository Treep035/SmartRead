using System.Collections.ObjectModel;

namespace SmartRead.MVVM.Models
{
    public class Category
    {
        // Nueva propiedad para alinear con la respuesta de la API
        public int IdCategory { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Book> Books { get; set; }

        // Constructor sin parámetros para la deserialización.
        public Category()
        {
            Books = new ObservableCollection<Book>();
        }

        // Constructor opcional para crear instancias con todos los valores
        public Category(int idCategory, string name, ObservableCollection<Book> books)
        {
            IdCategory = idCategory;
            Name = name;
            Books = books;
        }
    }
}
