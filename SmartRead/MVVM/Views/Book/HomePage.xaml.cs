using System.Collections.ObjectModel;

namespace SmartRead.MVVM.Views.Book
{
    public partial class HomePage : ContentPage
    {
        public ObservableCollection<Recommendation> Recommendations { get; set; }

        public HomePage()
        {
            InitializeComponent();

            Recommendations = new ObservableCollection<Recommendation>
            {
                new Recommendation { ImageSource = "image1.png" },
                new Recommendation { ImageSource = "image2.png" },
                new Recommendation { ImageSource = "image3.png" },
                new Recommendation { ImageSource = "image4.png" }
            };

            BindingContext = this;
        }
    }

    public class Recommendation
    {
        public string ImageSource { get; set; }
    }
}
