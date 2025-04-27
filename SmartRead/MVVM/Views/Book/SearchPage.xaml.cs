using SmartRead.MVVM.ViewModels; 
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Models;
using SmartRead.MVVM.Services;
using Microsoft.Extensions.Configuration;

namespace SmartRead.MVVM.Views.Book
{
    public partial class SearchPage : ContentPage
    {
        public SearchPage(AuthService authService, IConfiguration configuration)
        {
            InitializeComponent();
            BindingContext = new SearchViewModel(authService, configuration); 
        }
    }
}