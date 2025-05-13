namespace SmartRead.MVVM.Models
{
    public class UserPreferences
    {
        public string ColorTheme { get; set; } = "Claro";
        public string BackgroundColor { get; set; } = "#ffffff";
        public string TextColor { get; set; } = "#121212";
        public double FontSize { get; set; } = 16;
        public Dictionary<string, double> ReadingTimePerBook { get; set; } = new();
        public Dictionary<string, int> LastChapterIndexPerBook { get; set; } = new Dictionary<string, int>();
    }
}
