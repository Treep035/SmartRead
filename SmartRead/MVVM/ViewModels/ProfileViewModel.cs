using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace SmartRead.ViewModels
{
    public class ProfileViewModel
    {
        public ICommand TapGestureCommand { get; }
        public ObservableCollection<Book> LibrosQueTeHanGustado { get; set; }  // Libros normales (sin carrusel)
        public ObservableCollection<Category> Categories { get; set; }  // "Mi Lista" y "Leídos recientemente" (con carrusel)



        public ProfileViewModel()
        {
            TapGestureCommand = new Command(async () => await OnMenuClicked());

            // 🔹 SECCIÓN: "Libros que te han gustado" (SIN carrusel) 🔹
            LibrosQueTeHanGustado = new ObservableCollection<Book>
            {
                new Book { ImageSource = "libro1.png" }
            };



            // 🔹 SECCIÓN: "Mi Lista" y "Leídos recientemente" (CARRUSEL) 🔹
            Categories = new ObservableCollection<Category>
            {
                new Category("Mi Lista", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "libro2.jpg" },
                    new Book { ImageSource = "libro3.jpg" },
                    new Book { ImageSource = "libro4.jpg" },
                    new Book { ImageSource = "libro5.jpg" },
                    new Book { ImageSource = "libro6.jpg" }
                }),
                new Category("Leídos recientemente", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "leido1.png" },
                    new Book { ImageSource = "leido2.png" },
                    new Book { ImageSource = "leido3.jpg" }
                })
            };
        }

        

        private async Task OnMenuClicked()
        {
            await Application.Current.MainPage.DisplayAlert("Menú", "Aquí irá el menú lateral", "OK");
        }
    }
    public class Category
    {
        public string Name { get; set; }
        public ObservableCollection<Book> Books { get; set; }

        public Category(string name, ObservableCollection<Book> books)
        {
            Name = name;
            Books = books;
        }
    }

    public class Book
    {
        public string ImageSource { get; set; }
    }
}
