
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SmartRead.MVVM.ViewModels
{
    public partial class NewsViewModel : ObservableObject
    {
        public ObservableCollection<LibroNoticia> LibrosActuales { get; set; } = new ObservableCollection<LibroNoticia>();

        private bool _popularSelected = true;
        public bool PopularSelected
        {
            get => _popularSelected;
            set => SetProperty(ref _popularSelected, value);
        }

        private bool _novedadesSelected = false;
        public bool NovedadesSelected
        {
            get => _novedadesSelected;
            set => SetProperty(ref _novedadesSelected, value);
        }

        private readonly ObservableCollection<LibroNoticia> LibrosPopulares = new()
        {
            new LibroNoticia { Numero = "01", Imagen = "image2.png", Titulo = "Don Quijote de la Mancha", Descripcion = "La historia de un caballero que sueña con ser héroe y sus increibles aventuras en un mundo que ya no cree en la caballeria" },
            new LibroNoticia { Numero = "02", Imagen = "cien_anos.png", Titulo = "Cien años de soledad", Descripcion = "La saga de la familia Buendía en Macondo, un realismo mágico..." },
            new LibroNoticia { Numero = "03", Imagen = "el_principito.png", Titulo = "El Principito", Descripcion = "Un niño que viaja por planetas aprendiendo lecciones de vida..." }
        };

        private readonly ObservableCollection<LibroNoticia> NovedadesEditoriales = new()
        {
            new LibroNoticia { Numero = "", Imagen = "1984.jpg", Titulo = "1984", Descripcion = "Una sociedad controlada por el Gran Hermano..." },
            new LibroNoticia { Numero = "", Imagen = "fahrenheit.jpg", Titulo = "Fahrenheit 451", Descripcion = "Una sociedad donde los libros están prohibidos..." },
            new LibroNoticia { Numero = "", Imagen = "brave_new_world.jpg", Titulo = "Un Mundo Feliz", Descripcion = "Una distopía sobre el control social..." }
        };

        public NewsViewModel()
        {
            CargarPopulares();
        }

        [RelayCommand]
        public void CargarPopulares()
        {
            if (!PopularSelected)
            {
                PopularSelected = true;
                NovedadesSelected = false;
                LibrosActuales.Clear();
                foreach (var libro in LibrosPopulares)
                {
                    LibrosActuales.Add(libro);
                }
            }
        }

        [RelayCommand]
        public void CargarNovedades()
        {
            if (!NovedadesSelected)
            {
                PopularSelected = false;
                NovedadesSelected = true;
                LibrosActuales.Clear();
                foreach (var libro in NovedadesEditoriales)
                {
                    LibrosActuales.Add(libro);
                }
            }
        }
    }

    public class LibroNoticia
    {
        public string Numero { get; set; }
        public string Imagen { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
    }
}
