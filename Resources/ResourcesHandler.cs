using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using BlogExtensions.Extensions;
using Microsoft.Ajax.Utilities;

namespace Blog.Resources
{
    public class ResourcesHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        { 
            // Get a set of resources appropriate to the culture defined by the browser
            ResourceSet resourceSet = Index.ResourceManager.GetResourceSet
                (CultureInfo.CurrentUICulture, true, true);
            ResourceSet resourceSetAdmin = Index_Admin.ResourceManager.GetResourceSet
                (CultureInfo.CurrentUICulture, true, true);
            context.Response.ContentType = "text/javascript";

            var content = "var Resources = {};\n";
            foreach (DictionaryEntry res in resourceSet) {
                // Create a property on the javascript object for each text resource
                content += string.Format("Resources.{0} = '{1}';\n", res.Key, 
                    HttpUtility.JavaScriptStringEncode(res.Value.ToString()));
            }

            if (context.User.Identity.IsAuthenticated) { 
                foreach (DictionaryEntry res in resourceSetAdmin) {
                    content += string.Format("Resources.{0} = '{1}';\n", res.Key, 
                        HttpUtility.JavaScriptStringEncode(res.Value.ToString()));
                }
            }

            if (context.IsDebuggingEnabled)
            {
                context.Response.Write(content);
            }
            else
            {
                Minify(context.Response, content);
            }
         
            SetHeaders(context.Response);
        }

        private static void Minify(HttpResponse response, string content)
        { 
            Minifier minifier = new Minifier();
   
            CodeSettings settings = new CodeSettings() { PreserveImportantComments = false };
            response.Write(minifier.MinifyJavaScript(content, settings));
        }

        private static void SetHeaders(HttpResponse response)
        {
            response.ContentType = "text/javascript";

            response.Cache.SetLastModified(FileDateExtensions.GetLastWriteDate(Localization.Resources));
            response.Cache.SetValidUntilExpires(true);
            response.Cache.SetExpires(DateTime.Now.AddYears(1));
            response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
            response.Cache.SetVaryByCustom("Accept-Encoding");
        
            response.AddCacheDependency(new CacheDependency(Localization.Resources.Select(HostingEnvironment.MapPath).ToArray()));
        }
    
        public bool IsReusable { get; private set; }

    }
} 