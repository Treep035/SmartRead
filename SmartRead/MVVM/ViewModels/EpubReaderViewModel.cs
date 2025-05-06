using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using VersOne.Epub;

namespace SmartRead.MVVM.ViewModels
{
    public partial class EpubReaderViewModel : ObservableObject, IQueryAttributable
    {
        // EPUB cargado
        [ObservableProperty]
        private EpubBook epubBook;

        // HTML del capítulo actual
        [ObservableProperty]
        private string epubContentHtml;

        // Índice del capítulo actual (0-based)
        [ObservableProperty]
        private int currentChapter;

        // Instancias únicas de los comandos de navegación
        public IRelayCommand GoPreviousCommand { get; }
        public IRelayCommand GoNextCommand { get; }
        public ICommand ExitCommand { get; }

        public EpubReaderViewModel()
        {
            // Creamos los comandos pasándoles sus métodos y condiciones
            GoPreviousCommand = new RelayCommand(GoPrevious, CanGoPrevious);
            GoNextCommand = new RelayCommand(GoNext, CanGoNext);
            ExitCommand = new Command(async () => await Shell.Current.GoToAsync("//info"));

        }

        /// <summary>
        /// Se llama al navegar a esta página, recibe el parámetro epubBook.
        /// </summary>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("epubBook", out var epubBookObj) && epubBookObj is EpubBook book)
            {
                EpubBook = book;
                CurrentChapter = 0;
                UpdateHtml();
                // Aseguramos estado inicial de botones
                GoPreviousCommand.NotifyCanExecuteChanged();
                GoNextCommand.NotifyCanExecuteChanged();
            }
        }

        private void GoPrevious()
        {
            CurrentChapter--;
            UpdateHtml();
            GoPreviousCommand.NotifyCanExecuteChanged();
            GoNextCommand.NotifyCanExecuteChanged();
        }

        private void GoNext()
        {
            CurrentChapter++;
            UpdateHtml();
            GoPreviousCommand.NotifyCanExecuteChanged();
            GoNextCommand.NotifyCanExecuteChanged();
        }

        private bool CanGoPrevious() => EpubBook != null && CurrentChapter > 0;
        private bool CanGoNext() => EpubBook != null && CurrentChapter < (EpubBook.ReadingOrder?.Count ?? 0) - 1;

        /// <summary>
        /// Reconstruye el HTML para el capítulo currentChapter.
        /// </summary>
        private void UpdateHtml()
        {
            if (EpubBook?.ReadingOrder == null)
            {
                EpubContentHtml = string.Empty;
                return;
            }

            var chapter = EpubBook.ReadingOrder[CurrentChapter];
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html><head><meta charset=\"utf-8\"><style>");
            sb.AppendLine("body { font-family: sans-serif; line-height: 1.6; padding: 1em; }");
            sb.AppendLine("img { display: block; margin: 1em auto; max-width: 100% !important; height: auto !important; object-fit: contain; }");
            sb.AppendLine("footer { text-align: center; font-size: 0.9em; color: #666; margin-top: 2em; }");
            sb.AppendLine("</style></head><body>");

            string stripped = StripEpubStylesAndScripts(chapter.Content);
            string cleaned = RemoveImgDimensions(stripped);
            string inlined = InlineImagesInHtml(cleaned, EpubBook);
            sb.AppendLine(inlined);

            sb.AppendLine($"<footer><p>Capítulo {CurrentChapter + 1} de {EpubBook.ReadingOrder.Count}</p></footer>");
            sb.AppendLine("</body></html>");

            EpubContentHtml = sb.ToString();
        }

        // ===== Métodos auxiliares: =====

        // Elimina <script>, <style> y <link rel="stylesheet">
        private string StripEpubStylesAndScripts(string html)
        {
            html = Regex.Replace(html, "<script[^>]*>[\\s\\S]*?</script>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "<style[^>]*>[\\s\\S]*?</style>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, "<link[^>]*rel=[\"']stylesheet[\"'][^>]*>", "", RegexOptions.IgnoreCase);
            return html;
        }

        // Quita atributos width/height y estilos inline de ancho/alto en <img>
        private string RemoveImgDimensions(string html)
        {
            html = Regex.Replace(
                html,
                "(<img\\b[^>]*?)\\s+(?:width|height)=[\"'][^\"']*[\"']",
                "$1",
                RegexOptions.IgnoreCase);

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
                    return Regex.Replace(match.Value, "\\s*style=[\"'][^\"']*[\"']", "", RegexOptions.IgnoreCase);
                },
                RegexOptions.IgnoreCase);

            return html;
        }

        // Incrusta imágenes como Base64 dentro del HTML
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

        // Busca el recurso de imagen dentro del EPUB
        private EpubLocalByteContentFile? FindImage(EpubBook epubBook, string imagePath)
        {
            return epubBook.Content.Images.Local
                .FirstOrDefault(i => i.FilePath == imagePath)
                ?? epubBook.Content.Images.Local
                     .FirstOrDefault(i => i.FilePath == imagePath.TrimStart('.', '/'))
                ?? epubBook.Content.Images.Local
                     .FirstOrDefault(i => i.FilePath.EndsWith(imagePath.TrimStart('.', '/')));
        }

        // Devuelve el MIME type según la extensión del archivo
        private string GetMimeType(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream",
            };
        }
    }
}
