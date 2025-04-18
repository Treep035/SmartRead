using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using System.Windows.Input;

namespace SmartRead.MVVM.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        // Propiedades para enlazar con la vista
        [ObservableProperty]
        private string idioma = Preferences.Get("idioma", "Español");

        [ObservableProperty]
        private string tema = Preferences.Get("tema", "Oscuro");

        [ObservableProperty]
        private string tamañoLetra = Preferences.Get("tamañoLetra", "12.0");

        public SettingsViewModel()
        {
            // Cargar configuraciones al iniciar
            LoadSettings();
        }


        public List<string> ListaIdiomas { get; } = new()
{
    "Español",
    "Inglés"
};
        public List<string> ListaTemas { get; } = new()
{
    "Oscuro",
    "Claro"
};
        public List<string> ListaTamañosLetra { get; } = new()
{
    "12.0",
    "12.5"
};

        // Método para cargar las configuraciones (ya no necesitas manipular los Pickers directamente aquí)
        private void LoadSettings()
        {
            // Aquí ya no es necesario asignar directamente a los Pickers, ya que lo haremos mediante binding
            // Las propiedades ya están siendo cargadas con valores predeterminados en la declaración
        }

        // Método para guardar configuración de idioma
        [RelayCommand]
        private void OnIdiomaChangedCommand(string value)
        {
            Preferences.Set("idioma", value); // Guardar el valor seleccionado
            Idioma = value; // Actualiza la propiedad Idioma
        }

        // Método para guardar configuración de tema
        [RelayCommand]
        private void OnTemaChangedCommand(string value)
        {
            Preferences.Set("tema", value); // Guardar el valor seleccionado
            Tema = value; // Actualiza la propiedad Idioma
        }

        // Método para guardar configuración de tamaño de letra
        [RelayCommand]
        private void OnTamañoLetraChangedCommand(string value)
        {
            Preferences.Set("tamañoLetra", value); // Guardar el valor seleccionado
            TamañoLetra = value;
        }
    }
}
