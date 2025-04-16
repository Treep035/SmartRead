using CommunityToolkit.Maui;

using Microsoft.Extensions.Logging;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.ViewModels;
using SmartRead.MVVM.Views;
using SmartRead.MVVM.Views.Book;
using SmartRead.MVVM.Views.User;
using SmartRead.MVVM.Views.User.Account;
using SmartRead.MVVM.Views.User.Authentication;
using Microsoft.Extensions.Configuration;
using Xe.AcrylicView;


namespace SmartRead
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            AppContext.SetSwitch("System.Reflection.NullabilityInfoContext.IsSupported", true);
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseAcrylicView()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            using var stream = FileSystem.OpenAppPackageFileAsync("Properties/appsettings.json").GetAwaiter().GetResult();
            builder.Configuration.AddJsonStream(stream);

            // Registrar `AppShell`
            builder.Services.AddTransient<AppShell>();

            // Registrar ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();

            // Registrar Vistas
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<ForgotPasswordPage>();
            builder.Services.AddTransient<LoadingPage>();
            builder.Services.AddTransient<ProfilePage>();

            builder.Services.AddTransient<HomePage>();
            builder.Services.AddSingleton<HomeViewModel>();


            builder.Services.AddSingleton<NewsViewModel>();
            builder.Services.AddTransient<NewsPage>();
            



            // Registrar Servicios
            builder.Services.AddTransient<AuthService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
