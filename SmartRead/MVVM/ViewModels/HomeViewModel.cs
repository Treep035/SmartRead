using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using SmartRead.MVVM.Models;
using SmartRead.MVVM.Views.Book;
using CommunityToolkit.Maui.Views;

namespace SmartRead.MVVM.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        public ObservableCollection<Category> Categories { get; set; }

        public HomeViewModel()
        {
            Categories = new ObservableCollection<Category>
            {
                new Category("Recomendaciones para ti", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "image1.png" },
                    new Book { ImageSource = "image2.png" }
                }),
                new Category("Seguir leyendo", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "image3.png" },
                    new Book { ImageSource = "image4.png" }
                }),
                new Category("Aventuras", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "image5.png" },
                    new Book { ImageSource = "image6.png" }
                }),
                new Category("Fantástico", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "image7.png" },
                    new Book { ImageSource = "image8.png" }
                }),
                new Category("Intriga", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "image9.png" },
                    new Book { ImageSource = "image10.png" }
                }),
                new Category("Infantil y juvenil", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "image9.png" },
                    new Book { ImageSource = "image10.png" }
                }),
                new Category("Terror", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "image9.png" },
                    new Book { ImageSource = "image10.png" }
                }),
                new Category("Clásico", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "image9.png" },
                    new Book { ImageSource = "image10.png" }
                }),
                new Category("Ciencia ficción", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "image9.png" },
                    new Book { ImageSource = "image10.png" }
                }),
                new Category("Ciencia", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "image9.png" },
                    new Book { ImageSource = "image10.png" }
                }),
                new Category("Humor", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "image9.png" },
                    new Book { ImageSource = "image10.png" }
                }),
                new Category("Novela", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "image9.png" },
                    new Book { ImageSource = "image10.png" }
                }),
                new Category("Cuentos", new ObservableCollection<Book>
                {
                    new Book { ImageSource = "image9.png" },
                    new Book { ImageSource = "image10.png" }
                })
            };
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
            var categoriesPopup = new CategoriesPopup();  // Crear el Popup
            Application.Current.MainPage.ShowPopup(categoriesPopup);  // Mostrar el Popup
            return Task.CompletedTask;
        }
    }
}
