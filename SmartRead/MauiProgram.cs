using Microsoft.Extensions.Logging;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.ViewModels;
using SmartRead.MVVM.Views;
using SmartRead.MVVM.Views.Book;
using SmartRead.MVVM.Views.User;
using SmartRead.MVVM.Views.User.Authentication;

namespace SmartRead
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registrar `AppShell`
            builder.Services.AddTransient<AppShell>();

            // Registrar ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
  
            // Registrar Vistas
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<LoadingPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<HomePage>();

            // Registrar Servicios
            builder.Services.AddTransient<AuthService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
