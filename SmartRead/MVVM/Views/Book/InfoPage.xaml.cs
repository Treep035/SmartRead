using SmartRead.MVVM.Models;
using Microsoft.Maui.Controls;

namespace SmartRead.MVVM.Views.Book
{
    public partial class InfoPage : ContentPage
    {
        public InfoPage()
        {
            InitializeComponent();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("info") && query["info"] is Info info)
            {
                BindingContext = info;
            }
        }
    }
}
