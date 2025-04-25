using SmartRead.MVVM.ViewModels;
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Models;
using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.Services;

namespace SmartRead.MVVM.Views.Book
{
    [QueryProperty(nameof(Source), "source")]
    public partial class InfoPage : ContentPage
    {
        InfoPageViewModel vm;

        public string Source { get; set; } = "home"; 

        public InfoPage(AuthService authService, IConfiguration configuration)
        {
            InitializeComponent();

            vm = new InfoPageViewModel(authService, configuration);
            BindingContext = vm;
        }

        private async void ClosePopup(object sender, EventArgs e)
        {
            if (Source == "news")
                await Shell.Current.GoToAsync("//news");
            else
                await Shell.Current.GoToAsync("//home");
        }

        private async void OnValorarTapped(object sender, EventArgs e)
        {
            string opcion = await DisplayActionSheet(
                "¿Qué te parece este libro?",
                "Cancelar",
                null,
                "No es para mí",
                "Me gusta",
                "Me encanta"
            );

            if (opcion == "Cancelar" || opcion == null)
                return;

            int rating = opcion switch
            {
                "No es para mí" => 1,  
                "Me gusta" => 3,  
                "Me encanta" => 5,  
                _ => 1
            };

            await vm.SubmitReviewAsync(rating);
        }

    }
}
