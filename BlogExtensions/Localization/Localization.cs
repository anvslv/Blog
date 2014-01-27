using System.Collections;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Web;

namespace BlogExtensions.Localization
{
    public static class Localization
    {
        public static string CurrentLanguage
        {
            get { return Thread.CurrentThread.CurrentCulture.Name; }
        }

        public static string GetLocalResources()
        {
            ResourceSet resourceSetAdmin = Index_Admin.ResourceManager.GetResourceSet
                (CultureInfo.CurrentUICulture, true, true);
            var content = string.Empty;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                content = "var Resources = {};\n";
                foreach (DictionaryEntry res in resourceSetAdmin)
                {
                    content += string.Format("Resources.{0} = '{1}';\n", res.Key,
                        HttpUtility.JavaScriptStringEncode(res.Value.ToString()));
                }
            }
             
            return content;
        }

        public static string[] Resources
        {
            get { 
                return new[] {
                    "/resources/index.resx", 
                    "/resources/index.ru.resx", 
                    "/resources/index.admin.resx",
                    "/resources/index.admin.ru.resx",
                };
            }
        }
    }
} 
 