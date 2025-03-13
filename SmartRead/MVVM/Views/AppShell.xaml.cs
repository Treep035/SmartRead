using Microsoft.Maui.Controls;
using SmartRead.MVVM.Views.User.Account;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.Views.Book;
using SmartRead.MVVM.Views.User;

namespace SmartRead.MVVM.Views
{
    public partial class AppShell : Shell
    {
        private readonly AuthService authService;

        public AppShell()
        {
            InitializeComponent();

            authService = new AuthService();

            this.Navigating += OnNavigating;
        }

        private async void OnNavigating(object sender, ShellNavigatingEventArgs e)
        {
            var rutasProtegidas = new[] { nameof(HomePage), nameof(ProfilePage), nameof(NewsPage) };

            if (rutasProtegidas.Any(ruta => e.Target.Location.OriginalString.Contains(ruta)))
            {
                // Comprobaci�n sincr�nica del estado (sin delay)
                bool isAuthenticated = Preferences.Default.Get("AuthState", false);

                if (!isAuthenticated)
                {
                    e.Cancel();
                    await Shell.Current.GoToAsync("//login");
                }
            }
        }
    }
}
