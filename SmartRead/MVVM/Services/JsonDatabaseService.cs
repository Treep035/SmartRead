using System.Text.Json;
using SmartRead.MVVM.Models;

namespace SmartRead.MVVM.Services
{
    public class JsonDatabaseService
    {
        private readonly string _categoryCountsPath;
        private readonly string _categoriesPath;
        private readonly string _preferencesPath;

        public JsonDatabaseService(string basePath)
        {
            _categoryCountsPath = Path.Combine(basePath, "categoryCounts.json");
            _categoriesPath = Path.Combine(basePath, "categories.json");
            _preferencesPath = Path.Combine(basePath, "userPreferences.json");

            EnsureFileExists(_categoryCountsPath);
            EnsureFileExists(_categoriesPath);
            EnsureFileExists(_preferencesPath, defaultJson: JsonSerializer.Serialize(new UserPreferences()));
        }

        private void EnsureFileExists(string path, string defaultJson = "[]")
        {
            if (!File.Exists(path))
                File.WriteAllText(path, defaultJson);
        }

        // Incrementa contador de categoría
        public async Task AddCategoryTimeAsync(string category, int seconds)
        {
            var counts = await LoadCategoryCountsAsync();
            var entry = counts.FirstOrDefault(c => c.Category == category);
            if (entry != null)
            {
                entry.Seconds += seconds;
            }
            else
            {
                counts.Add(new CategoryCounter { Category = category, Seconds = seconds });
            }
            await SaveCategoryCountsAsync(counts);
        }

        // Guarda el contador de categorías
        private async Task SaveCategoryCountsAsync(List<CategoryCounter> counts)
        {
            var json = JsonSerializer.Serialize(counts);
            await File.WriteAllTextAsync(_categoryCountsPath, json);
        }

        // Carga el contador de categorías
        private async Task<List<CategoryCounter>> LoadCategoryCountsAsync()
        {
            var json = await File.ReadAllTextAsync(_categoryCountsPath);
            return JsonSerializer.Deserialize<List<CategoryCounter>>(json) ?? new();
        }

        // Obtiene las N categorías más populares
        public async Task<List<string>> GetRecommendedCategoriesAsync(int topN = 3)
        {
            var counts = await LoadCategoryCountsAsync();
            var top = counts
                .OrderByDescending(c => c.Seconds)
                .Take(topN)
                .Select(c => c.Category)
                .ToList();

            await SaveCategoriesAsync(top); // opcional
            return top;
        }

        // Guarda la lista de categorías recomendadas
        public async Task SaveCategoriesAsync(IEnumerable<string> categories)
        {
            var json = JsonSerializer.Serialize(categories);
            await File.WriteAllTextAsync(_categoriesPath, json);
        }

        // Carga la lista de categorías recomendadas
        public async Task<List<string>> LoadCategoriesAsync()
        {
            var json = await File.ReadAllTextAsync(_categoriesPath);
            return JsonSerializer.Deserialize<List<string>>(json) ?? new();
        }
        public async Task<UserPreferences> LoadPreferencesAsync()
        {
            var json = await File.ReadAllTextAsync(_preferencesPath);
            return JsonSerializer.Deserialize<UserPreferences>(json) ?? new UserPreferences();
        }

        public async Task SavePreferencesAsync(UserPreferences preferences)
        {
            var json = JsonSerializer.Serialize(preferences);
            await File.WriteAllTextAsync(_preferencesPath, json);
        }

        public async Task ResetPreferencesAsync()
        {
            var defaultPreferences = new UserPreferences(); // Preferencias por defecto
            await SavePreferencesAsync(defaultPreferences);
        }

    }
}
