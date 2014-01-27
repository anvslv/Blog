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
                "<img src=\"files/one.png\" alt=\"smth\" title=\"else\">" + 
                "<a href='files/two.png' class='without-caption image-link'>" +
                "   <img src=\"files/two.png\" alt=\"other\" title=\"stuff\">" +
                "</a>";

            Assert.Equal(expected, result);
        }
    }
}
