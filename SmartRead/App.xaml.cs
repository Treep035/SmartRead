using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.Views;
using SmartRead.MVVM.Views.Book;
using SmartRead.MVVM.Views.User;
using SmartRead.MVVM.Views.User.Authentication;
using Syncfusion.Licensing;
using Syncfusion.Maui.DataSource;

namespace SmartRead
{
    public partial class App : Application
    {
        private readonly IConfiguration _configuration;
        public App(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            InitializeComponent();

            _configuration = configuration;

            // Obtiene la clave de licencia desde appsettings.json
            var syncfusionLicenseKey = _configuration["Syncfusion:LicenseKey"];

            // Registra la licencia de Syncfusion
            if (!string.IsNullOrEmpty(syncfusionLicenseKey))
            {
                SyncfusionLicenseProvider.RegisterLicense(syncfusionLicenseKey);
            }

            MainPage = serviceProvider.GetRequiredService<AppShell>();

        }
    }
}
