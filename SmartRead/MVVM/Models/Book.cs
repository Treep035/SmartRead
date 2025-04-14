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
        /// Propiedad calculada que devuelve la URL de la imagen de portada.
        /// Se toma la ruta del FilePath quitando el nombre del fichero (si es que tiene una extensión reconocida)
        /// y concatenando "cover.jpg" más el token.
        ///
        /// Ejemplo esperado:
        /// https://smartreadfiles.blob.core.windows.net/book/A.%20A.%20Milne/El%20misterio%20de%20la%20Casa%20Roja%20(1197)/cover.jpg?si=smartread1&spr=https&sv=2022-11-02&sr=c&sig=...
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

                // Dividir el FilePath por el separador '/'
                string[] segments = FilePath.Split('/');
                Debug.WriteLine($"[CoverImageUrl] Número de segmentos encontrados: {segments.Length}");
                // Se espera al menos dos segmentos para obtener el directorio completo (por ejemplo, "Autor/Título (código)")
                if (segments.Length < 2)
                {
                    Debug.WriteLine("[CoverImageUrl] FilePath no tiene el formato esperado (menos de 2 segmentos).");
                    return null;
                }

                // Verificar si el último segmento es el nombre de un archivo reconocible.
                // En lugar de simplemente buscar un punto, se comprueba si finaliza con alguna de las extensiones conocidas.
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

                // Si se identifica que el último segmento es el nombre de un archivo, se extrae la parte de la ruta sin el nombre del archivo.
                string pathDirectory = FilePath;
                if (isFileName)
                {
                    int lastSlashIndex = FilePath.LastIndexOf('/');
                    pathDirectory = FilePath.Substring(0, lastSlashIndex);
                }
                Debug.WriteLine($"[CoverImageUrl] Directorio extraído: {pathDirectory}");

                // Construir la URL final
                string baseUrl = "https://smartreadfiles.blob.core.windows.net/book/";
                string token = "?si=smartread1&spr=https&sv=2022-11-02&sr=c&sig=6n0rP1a687TVDK5NiP5iuDYyqpx1XoLqXa%2BPuN8CNvA%3D";
                string finalUrl = baseUrl + pathDirectory + "/cover.jpg" + token;
                Debug.WriteLine($"[CoverImageUrl] URL generada: {finalUrl}");

                return finalUrl;
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

            // Si FilePath incluye el nombre del archivo, quitarlo
            int lastSlashIndex = FilePath.LastIndexOf('/');
            string pathDirectory = lastSlashIndex > 0 ? FilePath.Substring(0, lastSlashIndex) : FilePath;
            Debug.WriteLine($"[ParseAndSetAuthorTitleFromFilePath] Directorio: {pathDirectory}");

            // Dividir usando '/' como separador. Se espera al menos 2 partes: Autor y Título (con código)
            string[] parts = pathDirectory.Split('/');
            if (parts.Length >= 2)
            {
                this.Author = parts[0];
                string tituloConCodigo = parts[1];

                // Se busca el patrón " (" para quitar el código entre paréntesis
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
