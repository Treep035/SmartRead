using Microsoft.Maui.Controls;

namespace SmartRead.MVVM.Views
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Suscribirse al evento de navegaci�n
            this.Navigating += AppShell_Navigating;
        }

        private async void AppShell_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            // Si no ha iniciado sesi�n y se intenta navegar a una ruta protegida...
            if (!App.IsLoggedIn && IsProtectedRoute(e.Target))
            {
                e.Cancel(); // Cancela la navegaci�n
                await Shell.Current.GoToAsync("//login"); // Redirige al Login
            }
        }

        /// <summary>
        /// Considera como rutas p�blicas aquellas que contienen "login", "register" o "forgotpassword"
        /// </summary>
        private bool IsProtectedRoute(ShellNavigationState target)
        {
            var route = target.Location.OriginalString.ToLower();
            if (route.Contains("login") || route.Contains("register") || route.Contains("forgotpassword"))
                return false;
            return true;
        }
    }
}
