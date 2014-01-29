using System;
using System.IO;

namespace BlogExtensions.Extensions
{
    public static class FileNameExtensions
    {
        public static string AddSuffixAndReplaceExtension(this string path, string suffix, string ext = ".jpg")
        {
            return path.AddSuffix(suffix).ReplaceExtension(ext);
        }

        public static string AddSuffix(this string path, string suffix)
        {
            bool isUrl = path.Contains("/");
            string dir = Path.GetDirectoryName(path);

            if (dir == null)
            {
                throw new Exception("Filename is shitty.");
            }

            string fileName = Path.GetFileNameWithoutExtension(path);
            string fileExt = Path.GetExtension(path);
            var resultingPath = Path.Combine(dir, fileName + suffix + fileExt);
            if (isUrl)
                resultingPath = resultingPath.Replace("\\", "/");

            return resultingPath;
        }

        public static string ReplaceExtension(this string path, string extension)
        {
            bool isUrl = path.Contains("/");
            string dir = Path.GetDirectoryName(path);

            if (dir == null)
            {
                throw new Exception("Filename is shitty.");
            }

            string fileName = Path.GetFileNameWithoutExtension(path);
            
            var resultingPath = Path.Combine(dir, fileName + extension);
            if (isUrl)
                resultingPath = resultingPath.Replace("\\", "/");

            return resultingPath;
        }
    }
}