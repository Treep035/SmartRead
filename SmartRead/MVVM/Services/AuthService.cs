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
        public string GenerateRandomToken(int byteLength)
        {
            var random = new Random();
            var buffer = new byte[byteLength]; // 32 bytes (256 bits)
            random.NextBytes(buffer);
            return Convert.ToBase64String(buffer); // Convierte el buffer en una cadena Base64
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
            await SecureStorage.SetAsync("access_token", string.Empty);
            await SecureStorage.SetAsync("refresh_token", string.Empty);
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
