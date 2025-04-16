using SmartRead.MVVM.ViewModels; 
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Models; 

namespace SmartRead.MVVM.Views.Book
{
    public partial class InfoPage : ContentPage
    {
        public InfoPage()
        {
            InitializeComponent();
            BindingContext = new InfoPageViewModel(); 
        }
        private async void ClosePopup(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//home");
        }
    }
}