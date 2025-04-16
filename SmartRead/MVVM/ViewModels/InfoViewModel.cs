using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Models;

namespace SmartRead.MVVM.ViewModels
{
    public partial class InfoPageViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private Book book;

        // Comando para leer el libro y mostrar la URL
        public IRelayCommand ReadCommand { get; }

        public InfoPageViewModel()
        {
            ReadCommand = new RelayCommand(OnRead);
        }

        private async void OnRead()
        {
            // Muestra la URL del libro obtenida desde la propiedad FileUrl
            string message = Book?.FileUrl ?? "URL no disponible";
            await Shell.Current.DisplayAlert("URL del Libro", message, "OK");
            Debug.WriteLine(message);

        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("book", out var bookObj) && bookObj is Book book)
            {
                Book = book;
                Book.ParseAndSetAuthorTitleFromFilePath();
            }
        }
    }
}
