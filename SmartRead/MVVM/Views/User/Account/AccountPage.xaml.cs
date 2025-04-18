using CommunityToolkit.Mvvm.ComponentModel;
using SmartRead.MVVM.ViewModels;
using System.Diagnostics;

namespace SmartRead.MVVM.Views.User.Account;

public partial class AccountPage : ContentPage
{
    public AccountPage()
	{
		InitializeComponent();
        BindingContext = new AccountViewModel();
    }

    private async void Close(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//profile");
    }

}