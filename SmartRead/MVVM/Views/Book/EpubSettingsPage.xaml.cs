using CommunityToolkit.Maui.Views;
using SmartRead.MVVM.ViewModels;

namespace SmartRead.MVVM.Views.Book;

public partial class SettingsPopup : Popup
{
    public SettingsPopup()
    {
        InitializeComponent();

    }

    private void FontSizeSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        // Aquí actualizas el tamaño de fuente en el HTML del libro
        double newFontSize = e.NewValue;

        // Llamas al ViewModel para actualizar el tamaño de la fuente
        var viewModel = (EpubReaderViewModel)BindingContext;
        viewModel.UpdateFontSize(newFontSize);
    }

    private void ThemePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        string selectedTheme = picker.SelectedItem as string;

        if (selectedTheme == null)
            return;

        string backgroundColor = "#FFFFFF";
        string textColor = "#000000";
        string colorTheme = "Claro";

        if (selectedTheme == "Oscuro")
        {
            backgroundColor = "#121212";
            textColor = "#FFFFFF";
            colorTheme = "Oscuro";
        }
        if (selectedTheme == "Claro") // "Claro" u otro
        {
            backgroundColor = "#FFFFFF";
            textColor = "#000000";
            colorTheme = "Claro";
        }

        // Llamas al ViewModel para actualizar el tamaño de la fuente
        var viewModel = (EpubReaderViewModel)BindingContext;
        viewModel.UpdateColorTheme(backgroundColor, textColor, colorTheme);
    }

    private void Reset_Clicked(object sender, EventArgs e)
    {
        // Aquí llamas al ViewModel para restablecer la configuración
        var viewModel = (EpubReaderViewModel)BindingContext;
        viewModel.ResetAndApplyPreferencesAsync();
    }

    private void Close_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}
