using System;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace BlogExtensions.Extensions
{
    public static class FileDateExtensions { 
    
        public static DateTime GetLastWriteDate(string[] resources)
        {
            DateTime date = DateTime.MinValue;
            
            foreach (var resource in resources)
            {
                string relativeResources = VirtualPathUtility.ToAbsolute("~" + resource);
                string absoluteResources = HostingEnvironment.MapPath(relativeResources);
                var writeDate = File.GetLastWriteTime(absoluteResources);
                if (writeDate > date)
                    date = writeDate;
            }
            return date;
        }
    }
}