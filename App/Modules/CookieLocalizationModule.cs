// http://www.manas.com.ar/smedina/2008/12/17/internationalization-in-aspnet-mvc/

using System;
using System.Threading;
using System.Web;
using MiniBlog.App.Code;

namespace MiniBlog.App.Modules
{
    public class CookieLocalizationModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnContextBeginRequest;
        }

        private void OnContextBeginRequest(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Blog.Language))
            {
                var culture = new System.Globalization.CultureInfo(Blog.Language);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }

            // eat the cookie (if any) and set the culture
            else if (HttpContext.Current.Request.Cookies["language"] != null)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies["language"];
                string lang = cookie.Value;
                if (lang != "undefined")
                {
                    var culture = new System.Globalization.CultureInfo(lang);
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
            }
        }
    }
}