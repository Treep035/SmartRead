using System.Diagnostics;
using System.Text.Json;
using Microsoft.Maui.Storage;
using SmartRead.MVVM.Models;

namespace SmartRead.MVVM.Services
{
    public class JsonDatabaseService
    {
        private readonly string _categoryCountsPath;
        private readonly string _categoriesPath;
        private readonly string _preferencesPath;
        private readonly string _booksClickedPath;

        public JsonDatabaseService()
        {
            string basePath = Path.Combine(FileSystem.AppDataDirectory, "SmartReadData");

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            _categoryCountsPath = Path.Combine(basePath, "categoryCounts.json");
            _categoriesPath = Path.Combine(basePath, "categories.json");
            _preferencesPath = Path.Combine(basePath, "userPreferences.json");
            _booksClickedPath = Path.Combine(basePath, "booksClicked.json");

            EnsureFileExists(_categoryCountsPath);
            EnsureFileExists(_categoriesPath);
            EnsureFileExists(_preferencesPath, defaultJson: JsonSerializer.Serialize(new UserPreferences()));
            EnsureFileExists(_booksClickedPath);
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

        public async Task SaveIdBooksForRead(int id)
        {
            var ids = await GetIdBooksForRead();
            Debug.WriteLine("LLLLLLLLLLL" + ids);
            if (!ids.Contains(id))
            {
                ids.Add(id);
                var json = JsonSerializer.Serialize(ids);
                Debug.WriteLine("OOOOOOOOOO" + json);
                await File.WriteAllTextAsync(_booksClickedPath, json);
            }
        }

        public async Task<List<int>> GetIdBooksForRead()
        {
            if (!File.Exists(_booksClickedPath))
                return new List<int>();

            var json = await File.ReadAllTextAsync(_booksClickedPath);
            Debug.WriteLine("KKKKKKKKKKKKKK" + json);
            Debug.WriteLine("Ruta completa del archivo: " + _booksClickedPath);
            return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
        }

        public async Task SaveReadingTimeAsync(int bookId, TimeSpan sessionTime)
        {
            var prefs = await LoadPreferencesAsync();
            string id = bookId.ToString();

            double secondsToAdd = sessionTime.TotalSeconds;

            Debug.WriteLine($"Tiempo de lectura para el libro {bookId}: {secondsToAdd} segundos");

            prefs.ReadingTimePerBook[id] = secondsToAdd;

            await SavePreferencesAsync(prefs);
        }

        public async Task<List<int>> LoadBooksAndReadingTimeAsync()
        {
            var prefs = await LoadPreferencesAsync();

            // Tomamos los 5 IDs de libros con más tiempo de lectura
            var topBookIds = prefs.ReadingTimePerBook
                .Select(entry => new KeyValuePair<int, double>(int.Parse(entry.Key), entry.Value))
                .OrderByDescending(entry => entry.Value)
                .Take(5)
                .Select(entry => entry.Key)
                .ToList();

            Debug.WriteLine("HHHHHHHHH - Top 5 libros por tiempo de lectura:");
            foreach (var bookId in topBookIds)
            {
                Debug.WriteLine($"Libro ID: {bookId}");
            }

            return topBookIds;
        }
    }
}
