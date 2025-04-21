using System;
using System.IO;

namespace SmartRead.MVVM.Helpers
{
    public static class MimeHelper
    {
        public static string GetMimeType(string fileName)
        {
            string ext = Path.GetExtension(fileName)?.ToLowerInvariant();

            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
