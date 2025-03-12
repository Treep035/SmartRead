using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using SmartRead.MVVM.Models;
using SmartRead.MVVM.Views.Book;

namespace SmartRead.MVVM.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        public ObservableCollection<Recommendation> Recommendations { get; set; }

        public HomeViewModel()
        {
            Recommendations = new ObservableCollection<Recommendation>
            {
                new Recommendation { ImageSource = "image1.png" },
                new Recommendation { ImageSource = "image2.png" },
                new Recommendation { ImageSource = "image3.png" },
                new Recommendation { ImageSource = "image4.png" }
            };
        }

        [RelayCommand]
        public async Task NavigateToInfo(Recommendation recommendation)
        {
            var info = new Info 
            { 
                Title = "Título de ejemplo",
                Description = "Esta es una descripción de ejemplo",
                ImageSource = recommendation.ImageSource 
            };

            // Puedes pasar el recommendation como parámetro si es necesario
            await Shell.Current.Navigation.PushAsync(new InfoPage());

        }
    }
}

public class Recommendation
{
    public string ImageSource { get; set; }
}
