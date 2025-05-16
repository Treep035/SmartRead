using CommunityToolkit.Maui.Views;

namespace SmartRead.MVVM.Views.User.Account;

public partial class AboutPopup : Popup
{
    public AboutPopup()
    {
        InitializeComponent();
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }
}
