using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Services;

namespace SmartRead.MVVM.Views.User.Account
{
    public partial class ProfileDropDownPopup : Popup
    {
        private static bool _isPopupOpen;
        private bool _isClosing;
        private readonly AuthService _authService;

        // Propiedad pública para verificar el estado
        public static bool IsPopupOpen => _isPopupOpen;

        public ProfileDropDownPopup(AuthService authService)
        {
            InitializeComponent();
            SetPopupSize();
            _authService = authService;
            _isPopupOpen = true;
            this.Opened += OnPopupOpened;
            this.Closed += OnPopupClosed;
        }

        private void SetPopupSize()
        {
            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            this.Size = new Size(
                displayInfo.Width / displayInfo.Density,
                displayInfo.Height / displayInfo.Density
            );
        }

        private async void OnPopupOpened(object sender, PopupOpenedEventArgs e)
        {
            PanelBorder.TranslationY = 300;
            await PanelBorder.TranslateTo(0, 0, 300, Easing.CubicOut);
        }

        // Maneja el cierre desde cualquier método
        private void OnPopupClosed(object sender, PopupClosedEventArgs e)
        {
            _isPopupOpen = false;
        }

        private async Task ClosePopupAsync()
        {
            if (_isClosing) return;
            _isClosing = true;

            await PanelBorder.TranslateTo(0, 300, 250, Easing.CubicIn);

            try
            {
                Close();
            }
            catch (ObjectDisposedException)
            {
                // Ignorar si ya está cerrado
            }
            finally
            {
                _isClosing = false;
            }
        }
        private async void OnCloseTapped(object sender, EventArgs e)
        {
            await ClosePopupAsync();
        }
        private void OnEmptyTapped(object sender, EventArgs e)
        {
            // No hace nada, solo evita que el evento se propague al fondo
        }

        private async void OnSettingsTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//settings");
            await ClosePopupAsync();
        }

        private async void OnAccountTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//account");
            await ClosePopupAsync();
        }

        private async void OnLogoutTapped(object sender, EventArgs e)
        {
            // Eliminar los tokens de acceso y de actualización
            await _authService.ClearTokensAsync();

            // Redirigir a la página de inicio de sesión
            await Shell.Current.GoToAsync("//login");

            // Cerrar el popup
            await ClosePopupAsync();
        }
    }
}