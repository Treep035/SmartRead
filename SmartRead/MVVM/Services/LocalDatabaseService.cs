using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRead.MVVM.Models;

namespace MyMauiApp.Data
{
    // Clase que representa el servicio de base de datos local (SQLite).
    public class LocalDatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        // Constructor: establece la conexión con la base de datos SQLite.
        public LocalDatabaseService(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<Book>().Wait();        // Crea la tabla de libros (si no existe).
            _db.CreateTableAsync<UserClick>().Wait();   // Crea la tabla de registros de clics de libros (si no existe).
        }

        // Guarda múltiples libros en la base de datos.
        public Task<int> SaveBooksAsync(IEnumerable<Book> books)
            => _db.InsertAllAsync(books, "OR REPLACE");  // Utiliza "OR REPLACE" para reemplazar los registros existentes si ya existen.

        // Registra un click (cuando el usuario lee un libro).
        // category vendrá del Azure una vez lo implementen.
        public Task<int> AddClickAsync(int bookId, string category)
        {
            var click = new UserClick
            {
                BookId = bookId,
                Category = category,       // TODO: Azure deberá devolver esta categoría
                Timestamp = DateTime.UtcNow
            };
            return _db.InsertAsync(click);
        }

        // Obtiene las categorías más populares basadas en los clics del usuario.
        public async Task<List<string>> GetTopCategoriesAsync(int topN = 3)
        {
            var clicks = await _db.Table<UserClick>().ToListAsync();

            return clicks
                .GroupBy(c => c.Category)
                .OrderByDescending(g => g.Count())
                .Take(topN)
                .Select(g => g.Key)
                .ToList();
        }

        // Obtiene libros recomendados basados en las categorías más populares del usuario.
        public async Task<List<Book>> GetRecommendedBooksAsync(int maxPerCategory = 5)
        {
            var topCategories = await GetTopCategoriesAsync();
            var clickedBookIds = (await _db.Table<UserClick>().ToListAsync())
                .Select(c => c.BookId)
                .Distinct()
                .ToList();

            var recommendations = new List<Book>();
            foreach (var category in topCategories)
            {
                // TODO: Cuando Azure devuelva b.Category, descomenta y usa el Where
                var books = await _db.Table<Book>()
                    //.Where(b => b.Category == category && !clickedBookIds.Contains(b.IdBook))
                    .Where(b => !clickedBookIds.Contains(b.IdBook))   // Mientras tanto, solo excluimos leídos
                    .Take(maxPerCategory)
                    .ToListAsync();

                recommendations.AddRange(books);
            }

            return recommendations;
        }
    }
}
