using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VersOne.Epub;

namespace SmartRead.MVVM.ViewModels
{
    public partial class EpubReaderViewModel : ObservableObject, IQueryAttributable
    {
        // Objeto EpubBook recibido vía navegación
        [ObservableProperty]
        private EpubBook epubBook;

        // Contenido HTML generado a partir de las secciones (ReadingOrder) del EPUB
        [ObservableProperty]
        private string epubContentHtml;

        // Constructor sin parámetros para la instanciación desde XAML
        public EpubReaderViewModel()
        {
        }

        /// <summary>
        /// Recibe el parámetro "epubBook" al navegar a esta página y genera el HTML.
        /// </summary>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("epubBook", out var epubBookObj) && epubBookObj is EpubBook book)
            {
                EpubBook = book;
                GenerateEpubContentHtml();
            }
        }

        /// <summary>
        /// Genera el contenido HTML del EPUB combinando las secciones de lectura,
        /// eliminando estilos/scripts internos, incrustando imágenes como Base64 y aplicando estilos CSS propios.
        /// </summary>
        private void GenerateEpubContentHtml()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"utf-8\">");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: sans-serif; line-height: 1.6; padding: 1em; }");
            sb.AppendLine("img { display: block; margin: 1em auto; max-width: 100% !important; height: auto !important; width: auto; object-fit: contain; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            if (EpubBook?.ReadingOrder != null)
            {
                // Si no te interesa la portada, puedes hacer: .Skip(1) 
                foreach (var chapter in EpubBook.ReadingOrder)
                {
                    // 1) Quitar CSS/JS incrustado
                    string strippedHtml = StripEpubStylesAndScripts(chapter.Content);
                    // 2) Quitar atributos width/height de <img>
                    string cleanedHtml = RemoveImgDimensions(strippedHtml);
                    // 3) Incrustar imágenes como Base64
                    string inlinedHtml = InlineImagesInHtml(cleanedHtml, EpubBook);
                    sb.AppendLine(inlinedHtml);
                }
            }

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            EpubContentHtml = sb.ToString();
        }

        /// <summary>
        /// Elimina bloques <script>, <style> y <link rel="stylesheet"> del HTML.
        /// </summary>
        private string StripEpubStylesAndScripts(string html)
        {
            // Quita <script>…</script>
            html = Regex.Replace(html, "<script[^>]*>[\\s\\S]*?</script>", "", RegexOptions.IgnoreCase);
            // Quita <style>…</style>
            html = Regex.Replace(html, "<style[^>]*>[\\s\\S]*?</style>", "", RegexOptions.IgnoreCase);
            // Quita <link rel="stylesheet" …>
            html = Regex.Replace(html, "<link[^>]*rel=[\"']stylesheet[\"'][^>]*>", "", RegexOptions.IgnoreCase);
            return html;
        }

        /// <summary>
        /// Inserta imágenes como Base64 dentro del HTML.
        /// </summary>
        private string InlineImagesInHtml(string html, EpubBook epubBook)
        {
            var imageRegex = new Regex("<img[^>]+src=[\"']([^\"']+)[\"']", RegexOptions.IgnoreCase);

            return imageRegex.Replace(html, match =>
            {
                string tag = match.Value;
                string src = match.Groups[1].Value;

                var imageFile = FindImage(epubBook, src);
                if (imageFile != null)
                {
                    byte[] imageBytes = imageFile.Content;
                    string base64 = Convert.ToBase64String(imageBytes);
                    string mimeType = GetMimeType(src);

                    return tag.Replace(src, $"data:{mimeType};base64,{base64}");
                }

                return tag;
            });
        }

        /// <summary>
        /// Elimina atributos width, height y estilos inline de ancho/alto en etiquetas <img>.
        /// </summary>
        private string RemoveImgDimensions(string html)
        {
            // Quitar attributes width="..." y height="..."
            html = Regex.Replace(
                html,
                "(<img\\b[^>]*?)\\s+(?:width|height)=[\"'][^\"']*[\"']",
                "$1",
                RegexOptions.IgnoreCase);

            // Quitar declaraciones CSS inline de width/height dentro de style="..."
            html = Regex.Replace(
                html,
                "(<img\\b[^>]*\\bstyle=[\"'])([^\"']*)([\"'][^>]*>)",
                match =>
                {
                    string prefix = match.Groups[1].Value;
                    string styles = match.Groups[2].Value;
                    string suffix = match.Groups[3].Value;
                    var cleanStyles = string.Join(
                        "; ",
                        styles
                            .Split(';')
                            .Select(s => s.Trim())
                            .Where(s => !s.StartsWith("width", StringComparison.OrdinalIgnoreCase)
                                     && !s.StartsWith("height", StringComparison.OrdinalIgnoreCase))
                            .Where(s => s.Length > 0)
                    );
                    if (cleanStyles.Length > 0)
                    {
                        return $"{prefix}{cleanStyles}{suffix}";
                    }
                    // Si no queda nada, eliminamos todo el atributo style
                    return Regex.Replace(match.Value, "\\s*style=[\"'][^\"']*[\"']", "", RegexOptions.IgnoreCase);
                },
                RegexOptions.IgnoreCase);

            return html;
        }

        /// <summary>
        /// Busca la imagen en el contenido del EPUB con distintos enfoques.
        /// </summary>
        private EpubLocalByteContentFile? FindImage(EpubBook epubBook, string imagePath)
        {
            var image = epubBook.Content.Images.Local.FirstOrDefault(i => i.FilePath == imagePath);
            if (image != null)
                return image;

            string cleanedPath = imagePath.TrimStart('.', '/');
            image = epubBook.Content.Images.Local.FirstOrDefault(i => i.FilePath == cleanedPath);
            if (image != null)
                return image;

            return epubBook.Content.Images.Local.FirstOrDefault(i => i.FilePath.EndsWith(cleanedPath));
        }

        /// <summary>
        /// Devuelve el tipo MIME basado en la extensión del archivo.
        /// </summary>
        private string GetMimeType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
        }
    }
}
