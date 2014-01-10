using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using Blog.App.Code;
using BlogExtensions.Resources;

namespace Blog.App.Handlers
{
    public class PostHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
                throw new HttpException(403, "No access");

            string mode = context.Request.QueryString["mode"];
            string id = context.Request.Form["id"] ?? context.Request.QueryString["id"];
            if (mode == "saveImage")
            {
                SaveImage(context, id);
            }
            else if (mode == "getMarkdown")
            {
                GetMarkdown(context, id);
            } 
            else if (mode == "categories")
            {
                ReturnCategories(context);
            }
            else if (mode == "delete")
            {
                DeletePost(id);
            }
            else if (mode == "save")
            {
                EditPost(id, 
                    context.Request.Form["title"], 
                    context.Request.Form["content"],
                    bool.Parse(context.Request.Form["isPublished"]),
                    bool.Parse(context.Request.Form["isComments"]),
                    context.Request.Form["categories"].Split(','));
            }
        }

        private void SaveImage(HttpContext context, string id)
        { 
            if (context.Request.Files.Count == 0)
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("No files received.");
                context.Response.Write("{\"error\":\"Error while uploading file\"}");
            }
            else
            {
                HttpPostedFile uploadedfile = context.Request.Files[0];
                string fileName = Storage.SaveImage(id, uploadedfile);
                context.Response.ContentType = "text/plain";
                context.Response.Write("{\"filename\":\"" + fileName + "\"}");
            }
        }

        private void ReturnCategories(HttpContext context)
        {
            var list = new List<object>();
            var categories = Storage.GetAllPosts().SelectMany(p => p.Categories);

            foreach (string category in categories.Distinct())
            {
                list.Add(new { name = category });
            }

            context.Response.Write(new JavaScriptSerializer().Serialize(list));
            context.Response.Flush();
        }

        private void DeletePost(string id)
        {
            Post post = Storage.GetAllPosts().FirstOrDefault(p => p.ID == id);

            if (post == null)
                throw new HttpException(404, "The post does not exist");

            Storage.Delete(post);
        }

        private void GetMarkdown(HttpContext context, string id)
        {
            Post post = Storage.GetAllPosts().FirstOrDefault(p => p.ID == id);

            if (post != null)
            {    
                context.Response.Write(post.MarkdownContent);
            }
            else
            {
                context.Response.Write(string.Empty);
            }
        }

        private void EditPost(string id, string title, string content, bool isPublished, bool isComments, string[] categories)
        {
            Post post = Storage.GetAllPosts().FirstOrDefault(p => p.ID == id);

            if (post != null)
            {
                post.Title = title;
                post.MarkdownContent = content;
                post.TransformMarkdown();
                post.Categories = categories;
            }
            else
            {
                post = new Post { Title = title, MarkdownContent = content, 
                    Slug = CreateSlug(title), Categories = categories, ID = id};
       
                HttpContext.Current.Response.Write(post.Url);
            }

            if (categories.Length == 1 && categories[0] == string.Empty)
            {
                post.Categories = new List<string>().ToArray();
            }
         
            post.IsPublished = isPublished;
            post.IsComments = isComments;
        
            Storage.Save(post);
        }
     
        public static string CreateSlug(string title)
        {
            title = title.ToLowerInvariant().Replace(" ", "-");
            title = RemoveDiacritics(title);
            title = Regex.Replace(title, @"([^0-9a-z-])", string.Empty);

            if (Storage.GetAllPosts().Any(p => string.Equals(p.Slug, title)))
                throw new HttpException(409, Index_Admin.AlreadyInUse);

            return title.ToLowerInvariant();
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
} 