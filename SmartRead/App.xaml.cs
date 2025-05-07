using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using SmartRead.MVVM.Views.User.Authentication;
using Syncfusion.Licensing;
using System;

namespace SmartRead
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }
        private readonly IConfiguration _configuration;

        public App(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            InitializeComponent();
            Services = serviceProvider;
            _configuration = configuration;

            // Registra licencia Syncfusion si existe
            var key = _configuration["Syncfusion:LicenseKey"];
            if (!string.IsNullOrEmpty(key))
                SyncfusionLicenseProvider.RegisterLicense(key);

            // Empieza en la SplashPage
            MainPage = Services.GetRequiredService<SplashPage>();
        }
    }
}
