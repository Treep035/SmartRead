using CommunityToolkit.Maui.Views;

namespace SmartRead.MVVM.Views.User.Account;

public partial class PrivacyPolicyPopup : Popup
{
    public PrivacyPolicyPopup()
    {
        InitializeComponent();
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        Close();
    }
}
