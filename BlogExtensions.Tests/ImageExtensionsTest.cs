using System.IO;
using BlogExtensions.Extensions;
using Xunit;

namespace Tests
{ 
    public class ImageExtensionsTests
    {
        [Fact]
        public void LargeImagesWrapWithLink()
        {
            var input = "<img src=\"files/one.png\" alt=\"smth\" title=\"else\">" +
                        "<img src=\"files/two.png\" alt=\"other\" title=\"stuff\">";

            var result = input.ProcessLargeImages(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));
             
            var expected =
                "<img src=\"files/one-cmp-thumb.png\" alt=\"smth\" title=\"else\" width='272' height='260'>" +
                "<a href='files/two-cmp.png' class='without-caption image-link'>" +
                "   <img src=\"files/two-cmp-thumb.png\" alt=\"other\" title=\"stuff\" width='600' height='231'>" +
                "</a>";
            Assert.Equal(expected, result);
        }
    }
}
