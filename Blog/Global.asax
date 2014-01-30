<%@ Application Language="C#" %> 

<script RunAt="server">
 
    //public override string GetVaryByCustomString(HttpContext context, string arg)
    //{
    //    if (arg == "authenticated")
    //    {
    //        HttpCookie cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];

    //        if (cookie != null)
    //            return cookie.Value;
    //    }

    //    return base.GetVaryByCustomString(context, arg);
    //}

    public override string GetVaryByCustomString(HttpContext context, string arg)
    {
        string result = "";
        foreach (string customPart in arg.Split(';'))
        {
            switch (customPart)
            {
                case "authenticated":
                    HttpCookie cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
                    if (cookie != null)
                        result += cookie.Value;
                    break;
                case "language":
                    HttpCookie cookieLang = context.Request.Cookies["language"];
                    if (cookieLang != null)
                        result += cookieLang.Value;
                    break;
            }
        }

        if (!string.IsNullOrEmpty(result))
        {
            return result;
        }

        return base.GetVaryByCustomString(context, arg);
    } 
    
    public void Application_BeginRequest(object sender, EventArgs e)
    {
        Context.Items["IIS_WasUrlRewritten"] = "false"; 
    }
       
</script>
