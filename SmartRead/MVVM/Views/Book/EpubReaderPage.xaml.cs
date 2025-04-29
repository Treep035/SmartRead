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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var vm = BindingContext as EpubReaderViewModel;
            if (!string.IsNullOrWhiteSpace(vm?.EpubContentHtml))
            {
                epubWebView.Source = new HtmlWebViewSource
                {
                    Html = vm.EpubContentHtml
                };
            }
        }
    }
}
