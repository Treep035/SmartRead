using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Models;

namespace SmartRead.MVVM.ViewModels
{
    public class InfoPageViewModel : IQueryAttributable
    {
        // Propiedad para la información que se muestra en la página
        private Info _info;
        public Info Info
        {
            get => _info;
            set
            {
                _info = value;
                // Si usas INotifyPropertyChanged, notificar los cambios aquí
            }
        }

        // Comando para cerrar la página
        public ICommand CloseCommand { get; }

        public InfoPageViewModel()
        {
            CloseCommand = new Command(async () =>
            {
                try
                {
                    Console.WriteLine("Navegando hacia atrás...");
                    // Regresa a la página anterior en la pila de navegación
                    await Shell.Current.Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al intentar navegar hacia atrás: {ex.Message}");
                }
            });
        }

        // Manejo de parámetros pasados a la página (en este caso, la Info)
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("info") && query["info"] is string infoTitle)
            {
                Info = new Info { Title = infoTitle }; // O lo que necesites hacer con el parámetro
            }
        }
    }
}
