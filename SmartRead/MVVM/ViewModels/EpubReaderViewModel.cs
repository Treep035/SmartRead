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
            sb.AppendLine(@"
        html, body {
    margin: 0;
    padding: 0;
    overflow: hidden;
    height: 100%;
    width: 100%;
}

#container {
    column-width: 100vw;
    column-gap: 0;
    height: 100vh;
    width: 100vw;
    overflow-x: auto;
    overflow-y: hidden;
    white-space: normal; /* importante para que el contenido fluya verticalmente dentro de la página */
}

.page {
    display: inline-block;
    width: 100vw;
    height: 100vh;
    vertical-align: top;
    box-sizing: border-box;
    padding: 1em;
    font-family: sans-serif;
    font-size: 1.1em;
    line-height: 1.6;
    overflow-wrap: break-word;   /* <-- clave */
    word-break: break-word;      /* <-- clave */
    white-space: normal;         /* <-- para que no se quede en una sola línea */
}

        img {
            display: block;
            margin: 1em auto;
            max-width: 100% !important;
            height: auto !important;
        }
    ");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div id=\"container\">");

            if (EpubBook?.ReadingOrder != null)
            {
                foreach (var chapter in EpubBook.ReadingOrder)
                {
                    string strippedHtml = StripEpubStylesAndScripts(chapter.Content);
                    string cleanedHtml = RemoveImgDimensions(strippedHtml);
                    string inlinedHtml = InlineImagesInHtml(cleanedHtml, EpubBook);

                    // Insertar contenido como "páginas"
                    sb.AppendLine($"<div class=\"page\">{inlinedHtml}</div>");
                }
            }

            sb.AppendLine("</div>");
            sb.AppendLine(@"
        <script>
            let container = document.getElementById('container');
            document.body.addEventListener('click', function (e) {
                const x = e.clientX;
                const width = window.innerWidth;
                if (x < width * 0.3) {
                    container.scrollBy({ left: -width, behavior: 'smooth' });
                } else if (x > width * 0.7) {
                    container.scrollBy({ left: width, behavior: 'smooth' });
                }
            });
        </script>");
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
