using System;
using System.Collections.Generic;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.Models;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.ViewModels;
using Microsoft.Maui.Controls;

namespace SmartRead.MVVM.Views.Book
{
    public partial class CategoriesPopup : Popup
    {
        public CategoriesPopup(AuthService authService,
                               IConfiguration configuration,
                               IReadOnlyList<Category> existingCategories)
        {
            InitializeComponent();
            BindingContext = new CategoriesViewModel(authService, configuration, existingCategories);
        }

        private void ClosePopup(object sender, EventArgs e)
            => Close();

        private async void OnCategorySelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0)
                return;

            if (e.CurrentSelection[0] is Category selectedCategory)
            {
                Close();
                await Shell.Current.GoToAsync(
                    $"//home?selectedCategory={Uri.EscapeDataString(selectedCategory.Name)}");
            }
        }
    }
}
