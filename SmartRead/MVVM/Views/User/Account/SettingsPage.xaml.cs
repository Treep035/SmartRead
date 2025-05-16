using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartRead.MVVM.ViewModels;
using System.Diagnostics;
using SmartRead.MVVM.Views.User.Account;

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

    private void ShowPrivacyPolicy(object sender, EventArgs e)
    {
        this.ShowPopup(new PrivacyPolicyPopup());
    }

    private void ShowAbout(object sender, EventArgs e)
    {
        this.ShowPopup(new AboutPopup());
    }

}