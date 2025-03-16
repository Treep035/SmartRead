using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Views;
using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.Book
{
    public partial class CategoriesPopup : Popup
    {
        public CategoriesPopup()
        {
            InitializeComponent();
            BindingContext = new CategoriesViewModel();
        }

        // M�todo que cierra el popup cuando el bot�n es presionado
        private void ClosePopup(object sender, EventArgs e)
        {
            this.Close(); // M�todo que cierra el popup
        }
    }
}
