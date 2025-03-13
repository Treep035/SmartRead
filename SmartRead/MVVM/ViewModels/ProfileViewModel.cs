using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace PlatziTest.ViewModels
{
    public class ProfileViewModel
    {
        public ICommand TapGestureCommand { get; }

        public ProfileViewModel()
        {
            TapGestureCommand = new Command(async () => await OnMenuClicked());
        }

        private async Task OnMenuClicked()
        {
            await Application.Current.MainPage.DisplayAlert("Menú", "Aquí irá el menú lateral", "OK");
        }
    }
}
