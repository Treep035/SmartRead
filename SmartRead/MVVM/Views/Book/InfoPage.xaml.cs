using SmartRead.MVVM.ViewModels;
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Models;

namespace SmartRead.MVVM.Views.Book
{
    [QueryProperty(nameof(Source), "source")]
    public partial class InfoPage : ContentPage
    {
        public string Source { get; set; } = "home"; 

        public InfoPage()
        {
            InitializeComponent();
            BindingContext = new InfoPageViewModel();
        }

        private async void ClosePopup(object sender, EventArgs e)
        {
            if (Source == "news")
            {
                await Shell.Current.GoToAsync("//news");
            }
            else
            {
                await Shell.Current.GoToAsync("//home");
            }
        }
    }
}
