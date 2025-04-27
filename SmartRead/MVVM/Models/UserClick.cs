namespace SmartRead.MVVM.Models
{
    public class UserClick
    {
        public int BookId { get; set; }  // El ID del libro.
        public string Category { get; set; }  // La categoría del libro.
        public DateTime Timestamp { get; set; }  // La fecha y hora del clic.
    }
}
