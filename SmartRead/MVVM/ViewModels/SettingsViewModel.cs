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

        // Método para cargar las configuraciones (ya no necesitas manipular los Pickers directamente aquí)
        private void LoadSettings()
        {
            // Aquí ya no es necesario asignar directamente a los Pickers, ya que lo haremos mediante binding
            // Las propiedades ya están siendo cargadas con valores predeterminados en la declaración
        }

        // Método para guardar configuración de idioma
        [RelayCommand]
        private void OnIdiomaChanged()
        {
            Preferences.Set("idioma", idioma); // Guardar el valor seleccionado
        }

        // Método para guardar configuración de tema
        [RelayCommand]
        private void OnTemaChanged()
        {
            Preferences.Set("tema", tema); // Guardar el valor seleccionado
        }

        // Método para guardar configuración de tamaño de letra
        [RelayCommand]
        private void OnTamañoLetraChanged()
        {
            Preferences.Set("tamañoLetra", tamañoLetra); // Guardar el valor seleccionado
        }
    }
}
