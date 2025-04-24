using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.Book;

public partial class NewsPage : ContentPage
{
    public NewsPage(AuthService authService, IConfiguration configuration)
    {
		InitializeComponent();
        BindingContext = new NewsViewModel(authService, configuration);
	}
}