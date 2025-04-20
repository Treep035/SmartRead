using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.Models;
using SmartRead.MVVM.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace SmartRead.MVVM.ViewModels
{
    public partial class CategoriesViewModel : ObservableObject
    {
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Aquí reutilizamos las categorías que ya vienen cargadas.
        /// </summary>
        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        public CategoriesViewModel(AuthService authService,
                                   IConfiguration configuration,
                                   IReadOnlyList<Category> existingCategories)
        {
            _authService = authService;
            _configuration = configuration;

            // Copiamos directamente las categorías sin llamar a la API
            foreach (var cat in existingCategories)
                Categories.Add(cat);
        }
    }
}
