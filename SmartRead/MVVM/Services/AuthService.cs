using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRead.MVVM.Services
{
    public class AuthService
    {
        private const string AuthStateKey = "AuthState";

        public async Task<bool> IsAuthenticatedAsync()
        {
            // Delay para simular otras operaciones (por ejemplo, llamadas a la API)
            await Task.Delay(2000);

            var authState = Preferences.Default.Get<bool>(AuthStateKey, false);
            return authState;
        }
      

        public async Task SaveAccessTokenAsync(string accessToken)
        {
            await SecureStorage.SetAsync("access_token", accessToken);
        }

        // Método para recuperar el token de acceso
        public async Task<string> GetAccessTokenAsync()
        {
            var accessToken = await SecureStorage.GetAsync("access_token");
            return accessToken;
        }

        // Método para guardar el refresh token
        public async Task SaveRefreshTokenAsync(string refreshToken)
        {
            await SecureStorage.SetAsync("refresh_token", refreshToken);
        }

        // Método para recuperar el refresh token
        public async Task<string> GetRefreshTokenAsync()
        {
            var refreshToken = await SecureStorage.GetAsync("refresh_token");
            return refreshToken;
        }

        // Método para eliminar los tokens cuando el usuario cierra sesión
        public async Task ClearTokensAsync()
        {
            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");
        }

        public void Login()
        {
            Preferences.Default.Set<bool>(AuthStateKey, true);
        }

        public void Logout()
        {
            Preferences.Default.Remove(AuthStateKey);
        }
    }
}
