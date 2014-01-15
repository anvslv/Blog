using System.Web; 
using System.Web.Security; 

namespace MiniBlog.App.Handlers
{
    public class LoginHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (context.Request.HttpMethod == "POST")
            {
                string username = context.Request.Form["username"];
                string password = context.Request.Form["password"];
                string remember = context.Request.Form["remember"];

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    if (FormsAuthentication.Authenticate(username, password))
                    {
                        FormsAuthentication.SetAuthCookie(username, remember == "true");
                        context.Response.StatusCode = 200;
                    } 
                    else
                    {
                        context.Response.StatusCode = 401;
                        context.Response.SuppressFormsAuthenticationRedirect = true;
                        context.Response.End();
                    }
                }
                else if (!string.IsNullOrEmpty(context.Request.QueryString["signout"]))
                {
                    FormsAuthentication.SignOut();
                    context.Response.Redirect(context.Request.QueryString["ReturnUrl"], true);
                }
            }  
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
} 