using Microsoft.Maui.Controls;
using Microsoft.Extensions.Configuration;
using SmartRead.MVVM.Services;
using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.Book
{
    public partial class HomePage : ContentPage
    {
        private const double TopThreshold = 50;       // Umbral para forzar la expansión
        private const double ExpandedHeight = 60;       // Altura cuando se despliega la fila (igual al botón de Buscar)
        private const double CollapsedHeight = 0;       // Altura al colapsar la fila

        // Indica si la fila está colapsada (true) o expandida (false)
        private bool _isCollapsed = false;

        // Para detectar la dirección del scroll
        private double _previousScrollY = 0;

        public HomePage(AuthService authService, IConfiguration configuration)
        {
            InitializeComponent();
            BindingContext = new HomeViewModel(authService, configuration);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is HomeViewModel viewModel)
            {
                await viewModel.LoadCategoriesAsync();
            }
        }

        private void OnScrollViewScrolled(object sender, ScrolledEventArgs e)
        {
            // Si estamos cerca de la parte superior, forzamos la expansión
            if (e.ScrollY <= TopThreshold)
            {
                if (_isCollapsed)
                {
                    ExpandCategoriesRow();
                    _isCollapsed = false;
                }
            }
            else
            {
                // Detectamos la dirección del scroll:
                // Si se desplaza hacia abajo y no está colapsada, colapsamos
                if (e.ScrollY > _previousScrollY && !_isCollapsed)
                {
                    CollapseCategoriesRow();
                    _isCollapsed = true;
                }
                // Si se desplaza hacia arriba y está colapsada, expandimos
                else if (e.ScrollY < _previousScrollY && _isCollapsed)
                {
                    ExpandCategoriesRow();
                    _isCollapsed = false;
                }
            }
            _previousScrollY = e.ScrollY;
        }

        private void ExpandCategoriesRow()
        {
            var animation = new Animation(v => CategoriesButtonRow.HeightRequest = v,
                                          CategoriesButtonRow.HeightRequest,
                                          ExpandedHeight,
                                          Easing.CubicOut);
            animation.Commit(this, "ExpandCategoriesRow", 16, 250, finished: (v, c) => CategoriesButtonRow.HeightRequest = ExpandedHeight);
        }

        private void CollapseCategoriesRow()
        {
            var animation = new Animation(v => CategoriesButtonRow.HeightRequest = v,
                                          CategoriesButtonRow.HeightRequest,
                                          CollapsedHeight,
                                          Easing.CubicIn);
            animation.Commit(this, "CollapseCategoriesRow", 16, 250, finished: (v, c) => CategoriesButtonRow.HeightRequest = CollapsedHeight);
        }
    }
}
