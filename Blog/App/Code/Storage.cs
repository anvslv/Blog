using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MiniBlog.App.Code
{
    public static class Storage
    {
        private const string _rootFolderName = "/posts/";
        private static string _rootFolder = HostingEnvironment.MapPath("~" + _rootFolderName);

        public static List<Post> GetAllPosts()
        {
            if (HttpRuntime.Cache["posts"] == null)
                LoadPosts();

            if (HttpRuntime.Cache["posts"] != null)
            {
                return (List<Post>)HttpRuntime.Cache["posts"];
            }
            return new List<Post>();
        }

        public static void Save(Post post)
        {
            string folder = Path.Combine(_rootFolder, post.ID);
            string file = Path.Combine(folder, "post.xml");
            post.LastModified = DateTime.UtcNow;

            XDocument doc = new XDocument(
                new XElement("post",
                    new XElement("title", post.Title),
                    new XElement("slug", post.Slug),
                    new XElement("author", post.Author),
                    new XElement("pubDate", post.PubDate.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("lastModified", post.LastModified.ToString("yyyy-MM-dd HH:mm:ss")),
                    new XElement("content", post.MarkdownContent),
                    new XElement("renderedcontent", post.RenderedContent),
                    new XElement("ispublished", post.IsPublished),
                    new XElement("iscomments", post.IsComments),
                    new XElement("categories", string.Empty),
                    new XElement("comments", string.Empty)
                    ));

            XElement categories = doc.XPathSelectElement("post/categories");
            foreach (string category in post.Categories)
            {
                categories.Add(new XElement("category", category));
            }

            XElement comments = doc.XPathSelectElement("post/comments");
            foreach (Comment comment in post.Comments)
            {
                comments.Add(
                    new XElement("comment",
                        new XElement("author", comment.Author),
                        new XElement("email", comment.Email),
                        new XElement("website", comment.Website),
                        new XElement("ip", comment.Ip),
                        new XElement("userAgent", comment.UserAgent),
                        new XElement("date", comment.PubDate.ToString("yyyy-MM-dd HH:m:ss")),
                        new XElement("content", comment.Content),
                        new XAttribute("isAdmin", comment.IsAdmin),
                        new XAttribute("id", comment.ID)
                        ));
            }

            if (!File.Exists(file)) // New post
            {
                var posts = GetAllPosts();
                posts.Insert(0, post);
                posts.Sort((p1, p2) => p2.PubDate.CompareTo(p1.PubDate));
                HttpRuntime.Cache.Insert("posts", posts);
            }

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            doc.Save(file);
        }

        public static string SaveImage(string postId, HttpPostedFile uploadedfile)
        {
            var saveFolderPath = Path.Combine(_rootFolder,  postId);
            if (!Directory.Exists(saveFolderPath))
                Directory.CreateDirectory(saveFolderPath);
         
            string fileName = _rootFolderName + postId + "/" + uploadedfile.FileName;
            uploadedfile.SaveAs(saveFolderPath + "\\" + uploadedfile.FileName);
            return fileName;
     
        }
     
        public static void Delete(Post post)
        {
            var posts = GetAllPosts();
            string directory = Path.Combine(_rootFolder, post.ID);
            Directory.Delete(directory, true);
            posts.Remove(post);
        }

        private static void LoadPosts()
        {
            if (!Directory.Exists(_rootFolder))
                Directory.CreateDirectory(_rootFolder);

            List<Post> list = new List<Post>();

            foreach (string file in Directory.GetFiles(_rootFolder, "post.xml", SearchOption.AllDirectories))
            {
                XElement doc = XElement.Load(file);

                Post post = new Post()
                {
                    ID = Path.GetFileName( Path.GetDirectoryName( file ) ),
                    Title = ReadValue(doc, "title"),
                    Author = ReadValue(doc, "author"),
                    MarkdownContent = ReadValue(doc, "content"),
                    Slug = ReadValue(doc, "slug").ToLowerInvariant(),
                    PubDate = DateTime.Parse(ReadValue(doc, "pubDate")),
                    LastModified = DateTime.Parse(ReadValue(doc, "lastModified", DateTime.Now.ToString())),
                    IsPublished = bool.Parse(ReadValue(doc, "ispublished", "true")),
                    IsComments = bool.Parse(ReadValue(doc, "iscomments", "true")),
                };

                post.RenderedContent = ReadValue(doc, "renderedcontent", null);
                if (post.RenderedContent == null)
                    post.TransformMarkdown();

                LoadCategories(post, doc);
                LoadComments(post, doc);
                list.Add(post);
            }

            if (list.Count > 0)
            {
                list.Sort((p1, p2) => p2.PubDate.CompareTo(p1.PubDate));
                HttpRuntime.Cache.Insert("posts", list);
            }
        }

        private static void LoadCategories(Post post, XElement doc)
        {
            XElement categories = doc.Element("categories");
            if (categories == null)
                return;

            List<string> list = new List<string>();

            foreach (var node in categories.Elements("category"))
            {
                list.Add(node.Value);
            }

            post.Categories = list.ToArray();
        }

        private static void LoadComments(Post post, XElement doc)
        {
            var comments = doc.Element("comments");

            if (comments == null)
                return;

            foreach (var node in comments.Elements("comment"))
            {
                Comment comment = new Comment()
                {
                    ID = ReadAttribute(node, "id"),
                    Author = ReadValue(node, "author"),
                    Email = ReadValue(node, "email"),
                    Website = ReadValue(node, "website"),
                    Ip = ReadValue(node, "ip"),
                    UserAgent = ReadValue(node, "userAgent"),
                    IsAdmin = bool.Parse(ReadAttribute(node, "isAdmin", "false")),
                    Content = ReadValue(node, "content").Replace("\n", "<br />"),
                    PubDate = DateTime.Parse(ReadValue(node, "date", "2000-01-01")),
                };

                post.Comments.Add(comment);
            }
        }

        private static string ReadValue(XElement doc, XName name, string defaultValue = "")
        {
            if (doc.Element(name) != null)
                return doc.Element(name).Value;

            return defaultValue;
        }

        private static string ReadAttribute(XElement element, XName name, string defaultValue = "")
        {
            if (element.Attribute(name) != null)
                return element.Attribute(name).Value;

            return defaultValue;
        }
    }
} 