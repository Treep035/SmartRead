using Microsoft.Extensions.Logging;
using SmartRead.MVVM.ViewModels;
using SmartRead.MVVM.Views;
using SmartRead.MVVM.Views.User;

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



#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
