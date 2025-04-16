using SmartRead.MVVM.ViewModels;
using Microsoft.Maui.Controls;

namespace SmartRead.MVVM.Views.Book
{
    public partial class EpubReaderPage : ContentPage
    {
        public EpubReaderPage()
        {
            InitializeComponent();
            BindingContext = new EpubReaderViewModel();
        }
    }
}
