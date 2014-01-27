using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using MarkdownDeep;

namespace BlogExtensions.Extensions
{
    public static class TransformationExtensions
    {
        // Markdown parsing should be done. Markdown Extra is turned on. Also, 
        // if image is to big, it should be transformed from
        //      <img src="largeImage.png" alt="smth" />
        // to
        //      <a href="largeImage-cmp.png" class="without-caption image-link">
        //          <img src="smallImage-cmp-thumb.png" alt="smth" />
        //      </a>
        // for Magnific Popup, thus creating a compressed thumbnail and original file

        private const int maxWidth = 600;
        private static string dir;
        private static string suffixOrig = "-cmp";
        private static string suffixThumb = "-cmp-thumb";
        public static string TransformMarkdown(this string content, string directory)
        {
            var md = new Markdown
            {
                ExtraMode = true
            };

            return md.Transform(content).PerformAdditionalTransformations(directory);
        }

        private static string PerformAdditionalTransformations(this string content, string directory)
        {
            dir = directory;

            content = ProcessLargeImages(content);
            content = ProcessSmartyPants(content);
            
            return content;
        }

        private static string ProcessSmartyPants(string content)
        {
            return new SmartyPants().Transform(content, ConversionMode.EducateOldSchool);
        }

        public static string ProcessLargeImages(this string content, string directory)
        {
            dir = directory;
            return ProcessLargeImages(content);
        }

        private static string ProcessLargeImages(string content)
        {
            var regex = new Regex("<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase);
            foreach (Match match in regex.Matches(content))
            { 
                string src = match.Groups[1].Value;
                string wholeThing = match.Groups[0].Value;

                string path = Path.Combine(dir, src.TrimStart('/', '\\').Replace("/", "\\"));
                string pathThumb = path.AddSuffix(suffixThumb);
                string pathOrig = path.AddSuffix(suffixOrig);

                if (File.Exists(path))
                {
                    Image i = Image.FromFile(path);
                    var iWidth = i.Width;
                    var iHeight = i.Height;
                    i.Dispose();

                    if (iWidth > maxWidth)
                    {
                        double k = (double)iWidth/maxWidth; 
                        var newHeight = (int)((iHeight)/k);
                      
                        ImageUtilities.SaveImage(path, pathThumb, 70, maxWidth, newHeight);
                        ImageUtilities.SaveImage(path, pathOrig, 70, iWidth, iHeight);
                        
                        var wholeThingWithDimensions = wholeThing.Replace(src, src.AddSuffix(suffixThumb))
                            .Replace(">", " width='" + maxWidth + "' height='" + newHeight + "'>");

                        var link = @"<a href='" + src.AddSuffix(suffixOrig) +
                                  "' class='without-caption image-link'>   " + wholeThingWithDimensions + "</a>";
                        content = content.Replace(wholeThing, link);
                    }
                    else
                    {
                        var wholeThingWithDimensions = wholeThing.Replace(src, src.AddSuffix(suffixThumb))
                            .Replace(">", " width='" + iWidth + "' height='" + iHeight + "'>");
                         
                        ImageUtilities.SaveImage(path, pathThumb);
                        
                        content = content.Replace(wholeThing, wholeThingWithDimensions);

                    }
                }
            }
            return content;
        } 
    }
}