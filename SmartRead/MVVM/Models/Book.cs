using System;
using System.Diagnostics;

namespace SmartRead.MVVM.Models
{
    public class Book
    {
        public int IdBook { get; set; }
        public string Title { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string Author { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Propiedad calculada que devuelve la URL de la portada.
        /// </summary>
        public string CoverImageUrl
        {
            get
            {
                Debug.WriteLine($"[CoverImageUrl] FilePath original: {FilePath}");
                if (string.IsNullOrEmpty(FilePath))
                {
                    Debug.WriteLine("[CoverImageUrl] FilePath está vacío o es nulo.");
                    return null;
                }
                string[] segments = FilePath.Split('/');
                Debug.WriteLine($"[CoverImageUrl] Número de segmentos encontrados: {segments.Length}");
                if (segments.Length < 2)
                {
                    Debug.WriteLine("[CoverImageUrl] FilePath no tiene el formato esperado (menos de 2 segmentos).");
                    return null;
                }
                string lastSegment = segments[segments.Length - 1];
                string[] knownExtensions = { ".epub", ".pdf", ".mobi", ".azw3" };
                bool isFileName = false;
                foreach (var ext in knownExtensions)
                {
                    if (lastSegment.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                    {
                        isFileName = true;
                        break;
                    }
                }
                string pathDirectory = FilePath;
                if (isFileName)
                {
                    int lastSlashIndex = FilePath.LastIndexOf('/');
                    pathDirectory = FilePath.Substring(0, lastSlashIndex);
                }
                Debug.WriteLine($"[CoverImageUrl] Directorio extraído: {pathDirectory}");
                string baseUrl = "https://smartreadfiles.blob.core.windows.net/book/";
                string token = "?si=smartread1&spr=https&sv=2022-11-02&sr=c&sig=6n0rP1a687TVDK5NiP5iuDYyqpx1XoLqXa%2BPuN8CNvA%3D";
                string finalUrl = baseUrl + pathDirectory + "/cover.jpg" + token;
                Debug.WriteLine($"[CoverImageUrl] URL generada: {finalUrl}");
                return finalUrl;
            }
        }

        /// <summary>
        /// Propiedad calculada para formar la URL del libro.
        /// Se extrae el autor y el título (sin el código entre paréntesis) a partir del FilePath.
        /// Formato esperado:
        /// https://smartreadfiles.blob.core.windows.net/book/{Autor}/{Título con código}/{Título} - {Autor}.epub{token}
        /// Ejemplo:
        /// https://smartreadfiles.blob.core.windows.net/book/A. A. Milne/El misterio de la Casa Roja (1197)/El misterio de la Casa Roja - A. A. Milne.epub?si=smartread1&spr=https&sv=2022-11-02&sr=c&sig=...
        /// </summary>
        public string FileUrl
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    Debug.WriteLine("[FileUrl] FilePath está vacío o es nulo.");
                    return null;
                }
                // Se asume que el FilePath tiene el formato:
                // "Autor/Título con código/...archivo.ext"
                string[] segments = FilePath.Split('/');
                if (segments.Length < 2)
                {
                    Debug.WriteLine("[FileUrl] FilePath no tiene el formato esperado.");
                    return null;
                }
                // El primer segmento es el autor
                string author = segments[0];
                // El segundo segmento es el título con código, por ejemplo "El misterio de la Casa Roja (1197)"
                string folder = segments[1];
                // Extraer el título sin el código (quitando " (" y lo que sigue)
                int index = folder.IndexOf(" (");
                string title = index > 0 ? folder.Substring(0, index) : folder;
                Debug.WriteLine($"[FileUrl] Autor: {author}");
                Debug.WriteLine($"[FileUrl] Título extraído: {title}");

                string baseUrl = "https://smartreadfiles.blob.core.windows.net/book/";
                string token = "?si=smartread1&spr=https&sv=2022-11-02&sr=c&sig=6n0rP1a687TVDK5NiP5iuDYyqpx1XoLqXa%2BPuN8CNvA%3D";
                // Se utiliza el folder original (que incluye el código) para la URL
                string fileUrl = $"{baseUrl}{author}/{folder}/{title} - {author}.epub{token}";
                Debug.WriteLine($"[FileUrl] URL generada: {fileUrl}");
                return fileUrl;
            }
        }

        /// <summary>
        /// Método para extraer y asignar el Author y Title a partir del FilePath.
        /// Se espera el formato: "Autor/Título (código)/nombrearchivo.ext"
        /// </summary>
        public void ParseAndSetAuthorTitleFromFilePath()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                Debug.WriteLine("[ParseAndSetAuthorTitleFromFilePath] FilePath está vacío o es nulo.");
                return;
            }
            int lastSlashIndex = FilePath.LastIndexOf('/');
            string pathDirectory = lastSlashIndex > 0 ? FilePath.Substring(0, lastSlashIndex) : FilePath;
            Debug.WriteLine($"[ParseAndSetAuthorTitleFromFilePath] Directorio: {pathDirectory}");
            string[] parts = pathDirectory.Split('/');
            if (parts.Length >= 2)
            {
                this.Author = parts[0];
                string tituloConCodigo = parts[1];
                int posCodigo = tituloConCodigo.LastIndexOf(" (");
                this.Title = posCodigo > 0 ? tituloConCodigo.Substring(0, posCodigo) : tituloConCodigo;
                Debug.WriteLine($"[ParseAndSetAuthorTitleFromFilePath] Author asignado: {this.Author}");
                Debug.WriteLine($"[ParseAndSetAuthorTitleFromFilePath] Title asignado: {this.Title}");
            }
            else
            {
                Debug.WriteLine($"[ParseAndSetAuthorTitleFromFilePath] Formato inesperado en FilePath: {FilePath}");
            }
        }
    }
}
