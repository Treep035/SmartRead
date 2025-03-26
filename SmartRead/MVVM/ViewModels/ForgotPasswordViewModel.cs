using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartRead.MVVM.Views.User;
using SmartRead.MVVM.Views.Book;
using SmartRead.MVVM.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using System.Text.Json;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace SmartRead.MVVM.ViewModels
{
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        private readonly IConfiguration _configuration;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string code;

        public ForgotPasswordViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [RelayCommand]
        public async Task SendCodeToEmail()
        {

            if (string.IsNullOrWhiteSpace(Email))
            {
                await Shell.Current.DisplayAlert("Error", "Debe ingresar un correo para enviar el código.", "OK");
                return;
            }

            try
            {
                using var client = new HttpClient();
                var functionKey = _configuration["AzureFunctionKey"]; // Clave de la Azure Function
                var url = $"https://functionappsmartread20250303123217.azurewebsites.net/api/Function?code={functionKey}&action=sendcode";

                // Crear el JSON con el email
                var data = new { Email = Email };
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Enviar la solicitud POST a Azure
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    // Obtener el código de la respuesta
                    //string responseBody = await response.Content.ReadAsStringAsync();
                    //var result = JsonSerializer.Deserialize<RecoveryResponse>(responseBody);
                    //string recoveryCode = result?.Code;
                    //await Shell.Current.DisplayAlert("Éxito", $"Código recibido: {responseBody}", "OK");
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var recoveryResponse = JsonSerializer.Deserialize<RecoveryResponse>(responseBody, options);

                    if (recoveryResponse == null || string.IsNullOrEmpty(recoveryResponse.Code))
                    {
                        await Shell.Current.DisplayAlert("Error", "La respuesta no contiene un código válido.", "OK");
                        return;
                    }

                    await Shell.Current.DisplayAlert("Éxito", $"Código recibido: {recoveryResponse.Code}", "OK");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            } 

            try
            {
                string smtpServer = "smtp.gmail.com"; // Servidor SMTP (Gmail en este caso)
                int smtpPort = 587; // Puerto SMTP (587 para TLS)
                string smtpUser = "smartreadteam@gmail.com"; // Tu correo
                string smtpPassword = _configuration["SmtpPassword"]; // Tu contraseña o App Password

                // Crear mensaje de correo
                MailMessage mail = new()
                {
                    From = new MailAddress(smtpUser),
                    Subject = "Código de recuperación",
                    Body = "Tu código de recuperación es: 123456",
                    IsBodyHtml = false
                };
                mail.To.Add(Email);

                // Configurar SMTP
                using SmtpClient smtpClient = new(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPassword),
                    EnableSsl = true
                };

                // Enviar el correo
                await smtpClient.SendMailAsync(mail);

                await Shell.Current.DisplayAlert("Éxito", "Código enviado exitosamente a tu correo.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"2Ocurrió un error: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task SendCode()
        {

            if (string.IsNullOrWhiteSpace(Code))
            {
                await Shell.Current.DisplayAlert("Error", "Debe ingresar un código.", "OK");
                return;
            }

            await Shell.Current.DisplayAlert("Éxito", "Código correcto", "OK");
            //await Shell.Current.GoToAsync("//changepassword");
        }

        [RelayCommand]
        public async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}
