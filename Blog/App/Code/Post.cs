using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Hosting;
using BlogExtensions.Extensions;
using CookComputing.XmlRpc;

namespace MiniBlog.App.Code
{
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public class Post
    {
        public Post()
        {
            Title = "";
            ID = Guid.NewGuid().ToString();
            Author = HttpContext.Current.User.Identity.Name;
            PubDate = DateTime.UtcNow;
            LastModified = DateTime.UtcNow;
            Categories = new string[0];
            Comments = new List<Comment>();
            IsPublished = true;
            IsComments = true;
        }

        [XmlRpcMember("postid")]
        public string ID { get; set; }

        [XmlRpcMember("title")]
        public string Title { get; set; }

        [XmlRpcMember("author")]
        public string Author { get; set; }

        [XmlRpcMember("wp_slug")]
        public string Slug { get; set; }

        [XmlRpcMember("markdown")]
        public string MarkdownContent { get; set; }

        [XmlRpcMember("description")]
        public string RenderedContent { get; set; }

        public void TransformMarkdown()
        {
            RenderedContent = MarkdownContent.TransformMarkdown(HostingEnvironment.MapPath("~"));
        }

        [XmlRpcMember("dateCreated")]
        public DateTime PubDate { get; set; }

        [XmlRpcMember("dateModified")]
        public DateTime LastModified { get; set; }

        public bool IsPublished { get; set; }

        public bool IsComments { get; set; }

        [XmlRpcMember("categories")]
        public string[] Categories { get; set; }

        public List<Comment> Comments { get; private set; }

        public Uri AbsoluteUrl
        {
            get
            {
                Uri requestUrl = HttpContext.Current.Request.Url;
                return new Uri(requestUrl.Scheme + "://" + requestUrl.Authority + Url, UriKind.Absolute);
            }
        }

        public Uri Url
        {
            get { return new Uri(VirtualPathUtility.ToAbsolute("~/log/" + Slug), UriKind.Relative); }
        }

        public bool AreCommentsOpen(HttpContextBase context)
        {
            return IsComments &&
                   (PubDate > DateTime.UtcNow.AddDays(-Code.Blog.DaysToComment) ||
                    context.User.Identity.IsAuthenticated);
        }
    }
}