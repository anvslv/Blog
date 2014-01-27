using System.Web;
using Microsoft.Ajax.Utilities;

namespace BlogExtensions.Extensions
{
    public static class Minification
    { 
        public static string Minify(string content)
        {
            if (HttpContext.Current.IsDebuggingEnabled)
            {
                return content;
            }
            else
            {
                Minifier minifier = new Minifier();
                CodeSettings settings = new CodeSettings { PreserveImportantComments = false };

                return minifier.MinifyJavaScript(content, settings) + ";";
            }
        }
    }
}