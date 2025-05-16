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

            _preferencesPath = Path.Combine(basePath, "userPreferences.json");
            _booksClickedPath = Path.Combine(basePath, "booksClicked.json");

            EnsureFileExists(_preferencesPath, defaultJson: JsonSerializer.Serialize(new UserPreferences()));
            EnsureFileExists(_booksClickedPath);
        }

        private void EnsureFileExists(string path, string defaultJson = "[]")
        {
            if (!File.Exists(path))
                File.WriteAllText(path, defaultJson);
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
            if (!ids.Contains(id))
            {
                ids.Add(id);
                var json = JsonSerializer.Serialize(ids);
                await File.WriteAllTextAsync(_booksClickedPath, json);
            }
        }

        public async Task<List<int>> GetIdBooksForRead()
        {
            if (!File.Exists(_booksClickedPath))
                return new List<int>();

            var json = await File.ReadAllTextAsync(_booksClickedPath);
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

            if (prefs?.ReadingTimePerBook == null || prefs.ReadingTimePerBook.Count == 0)
            {
                Debug.WriteLine("No hay tiempos de lectura registrados. Devolviendo IDs por defecto.");
                return new List<int> { 1, 2, 3, 4, 5 };
            }
            // Tomamos los 5 IDs de libros con más tiempo de lectura
            var topBookIds = prefs.ReadingTimePerBook
                .Select(entry => new KeyValuePair<int, double>(int.Parse(entry.Key), entry.Value))
                .OrderByDescending(entry => entry.Value)
                .Take(5)
                .Select(entry => entry.Key)
                .ToList();

            return topBookIds;
        }
    }
}
