using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Models;

namespace SmartRead.MVVM.ViewModels
{
    // Hereda de ObservableObject para disponer de la implementación de INotifyPropertyChanged
    public partial class InfoPageViewModel : ObservableObject, IQueryAttributable
    {
        // Con el atributo ObservableProperty se genera la propiedad Book y se notifica los cambios
        [ObservableProperty]
        private Book book;

        // Definición del comando para cerrar la página, utilizando RelayCommand
        public IRelayCommand CloseCommand { get; }

        public InfoPageViewModel()
        {
          
        }

        // Se recibe el parámetro 'book' desde la navegación mediante la Query
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("book", out var bookObj) && bookObj is Book book)
            {
                // Asignamos el valor recibido
                Book = book;

                // Si es necesario, se puede procesar la ruta del fichero para obtener Author y Title
                Book.ParseAndSetAuthorTitleFromFilePath();
            }
        }
    }
}
