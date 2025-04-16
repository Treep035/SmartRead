using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Text;
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
        /// Genera el contenido HTML del EPUB combinando las secciones de lectura.
        /// Este método simplemente concatena el contenido de cada capítulo.
        /// </summary>
        private void GenerateEpubContentHtml()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html><head><meta charset=\"utf-8\"></head><body>");

            if (EpubBook?.ReadingOrder != null)
            {
                foreach (var chapter in EpubBook.ReadingOrder)
                {
                    // Simplemente se concatena el contenido HTML de cada capítulo.
                    sb.AppendLine(chapter.Content);
                }
            }
            sb.AppendLine("</body></html>");

            EpubContentHtml = sb.ToString();
        }
    }
}
