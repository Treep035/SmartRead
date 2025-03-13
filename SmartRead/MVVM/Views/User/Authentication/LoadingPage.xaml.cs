using SmartRead.MVVM.Services;
using SmartRead.MVVM.Views.Book;

namespace SmartRead.MVVM.Views.User.Authentication;

public partial class LoadingPage : ContentPage
{
    private readonly AuthService _authService;

    public LoadingPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (await _authService.IsAuthenticatedAsync())
        {
            // User is logged in
            // redirect to main page
            await Shell.Current.GoToAsync($"//home");
        }
        else
        {
            // User is not logged in 
            // Redirect to LoginPage  
            await Shell.Current.GoToAsync("//login");
        }
    }
}