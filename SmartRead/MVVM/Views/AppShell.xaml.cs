using Microsoft.Maui.Controls;

namespace SmartRead.MVVM.Views
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Suscribirse al evento de navegación
            this.Navigating += AppShell_Navigating;
        }

        private async void AppShell_Navigating(object sender, ShellNavigatingEventArgs e)
        {
            // Si no ha iniciado sesión y se intenta navegar a una ruta protegida...
            if (!App.IsLoggedIn && IsProtectedRoute(e.Target))
            {
                e.Cancel(); // Cancela la navegación
                await Shell.Current.GoToAsync("//login"); // Redirige al Login
            }
        }

        /// <summary>
        /// Considera como rutas públicas aquellas que contienen "login", "register" o "forgotpassword"
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
