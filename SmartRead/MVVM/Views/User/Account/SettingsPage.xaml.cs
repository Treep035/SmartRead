using CommunityToolkit.Mvvm.ComponentModel;
using SmartRead.MVVM.ViewModels;
using System.Diagnostics;

namespace SmartRead.MVVM.Views.User.Account;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
	{
		InitializeComponent();
    }

    private async void Close(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//profile");
    }

}