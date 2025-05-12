using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.User;

public partial class PaymentPage : ContentPage
{
	public PaymentPage()
	{
		InitializeComponent();
        BindingContext = new PaymentViewModel();

    }
}