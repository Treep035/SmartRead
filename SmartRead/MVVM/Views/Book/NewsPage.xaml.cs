using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.Book;

public partial class NewsPage : ContentPage
{
    public NewsPage()
    {
		InitializeComponent();
        BindingContext = new NewsViewModel();
	}
}