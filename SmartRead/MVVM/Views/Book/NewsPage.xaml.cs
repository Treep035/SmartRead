using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.Book
{
    public partial class NewsPage : ContentPage
    {
        private readonly NewsViewModel _viewModel;

        public NewsPage(AuthService authService, IConfiguration configuration)
        {
            InitializeComponent();
            _viewModel = new NewsViewModel(authService, configuration);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Si no hay datos cargados o simplemente para forzar recarga:
            if (!_viewModel.LibrosActuales.Any())
            {
                // Disparamos el comando sincrónicamente (la AsyncRelayCommand se encarga del Task interno)
                _viewModel.CargarPopularesCommand.Execute(null);
            }
        }
    }
}
