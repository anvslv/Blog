using System.Threading;

namespace Blog.Resources
{
    public static class Localization
    {
        public static string CurrentLanguage
        {
            get { return Thread.CurrentThread.CurrentCulture.Name; }
        }

        public static string[] Resources
        {
            get { return new[] {
                "/resources/index.resx", 
                "/resources/index.ru.resx", 
                "/resources/index.admin.resx",
                "/resources/index.admin.ru.resx",
            };
            }
        }
    }
} 
 