using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.Configuration;

namespace SmartRead.MVVM.Services
{
    public class AuthService
    {
        private const string AuthStateKey = "AuthState";
        private const string FunctionBaseUrl = "https://functionappsmartread20250303123217.azurewebsites.net/api/Function";

        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Comprueba si el usuario está autenticado:
        /// valida si existe un access token y lo comprueba mediante la API.
        /// Se actualiza el estado de autenticación y se retorna true o false según corresponda.
        /// </summary>
        public async Task<bool> IsAuthenticatedAsync()
        {
            ClearTokensAsync();

            // Recuperar el token de acceso almacenado.
            var accessToken = await GetAccessTokenAsync();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Preferences.Default.Set(AuthStateKey, false);
                return false;
            }

            // Obtener la clave de la función desde la configuración.
            string functionKey = _configuration["AzureFunctionKey"];
            if (string.IsNullOrWhiteSpace(functionKey))
            {
                Preferences.Default.Set(AuthStateKey, false);
                return false;
            }

            // Construir la URL para validar el token (acción "validate").
            string url = $"{FunctionBaseUrl}?code={functionKey}" +
                         $"&action=validate&accesstoken={Uri.EscapeDataString(accessToken)}";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        Preferences.Default.Set(AuthStateKey, false);
                        return false;
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    // Se espera que la API retorne "true" o "false" en texto plano.
                    bool isValid = bool.Parse(responseContent);
                    Preferences.Default.Set(AuthStateKey, isValid);
                    return isValid;
                }
            }
            catch (Exception)
            {
                Preferences.Default.Set(AuthStateKey, false);
                return false;
            }
        }

        public async Task SaveAccessTokenAsync(string accessToken)
        {
            await SecureStorage.SetAsync("access_token", accessToken);
        }

        // Método para recuperar el token de acceso.
        public async Task<string> GetAccessTokenAsync()
        {
            var accessToken = await SecureStorage.GetAsync("access_token");
            return accessToken;
        }

        public async Task SaveRefreshTokenAsync(string refreshToken)
        {
            await SecureStorage.SetAsync("refresh_token", refreshToken);
        }

        // Método para recuperar el refresh token.
        public async Task<string> GetRefreshTokenAsync()
        {
            var refreshToken = await SecureStorage.GetAsync("refresh_token");
            return refreshToken;
        }

        // Método para eliminar los tokens cuando el usuario cierra sesión.
        public async Task ClearTokensAsync()
        {
            SecureStorage.Remove("access_token");
            SecureStorage.Remove("refresh_token");

            Preferences.Default.Remove(AuthStateKey);

        }

        public void Login()
        {
            Preferences.Default.Set<bool>(AuthStateKey, true);
        }

        public void Logout()
        {
        }
    }
}