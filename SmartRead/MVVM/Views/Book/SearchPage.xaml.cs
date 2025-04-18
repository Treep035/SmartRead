using SmartRead.MVVM.ViewModels; 
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Models; 

namespace SmartRead.MVVM.Views.Book
{
    public partial class SearchPage : ContentPage
    {
        public SearchPage()
        {
            InitializeComponent();
            BindingContext = new SearchViewModel(); 
        }
        private async void Close(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//home");
        }
    }
}