using System.Collections.Generic;
using System.IO;
using BlogExtensions.Extensions;
using Xunit;

namespace Tests
{ 
    public class ImageExtensionsTests
    {
        private string one = "files/one{0}.png";
        private string two = "files/two{0}.jpg";
        private string three = "files/three{0}.png"; 

        private const string suffixOrig = "-cmp";
        private const string suffixThumb = "-cmp-thumb";

        private readonly string onePathOrig = "";
        private readonly string twoPathOrig = "";
        private readonly string threePathOrig = "";
     
        public ImageExtensionsTests()
        {
            string onePath = "";
            string oneThumbPath = "";
            string twoPath = "";
            string twoThumbPath = "";
            string threePath = "";
            string threeThumbPath = "";

            onePathOrig = Path.Combine("..\\..\\", string.Format(one, ""));
            twoPathOrig = Path.Combine("..\\..\\", string.Format(two, ""));
            threePathOrig = Path.Combine("..\\..\\", string.Format(three, ""));
            onePath = Path.Combine("..\\..\\", string.Format(one, suffixOrig));
            oneThumbPath = Path.Combine("..\\..\\", string.Format(one, suffixThumb));
            twoPath = Path.Combine("..\\..\\", string.Format(two, suffixOrig));
            twoThumbPath = Path.Combine("..\\..\\", string.Format(two, suffixThumb));
            threePath = Path.Combine("..\\..\\", string.Format(three, suffixOrig));
            threeThumbPath = Path.Combine("..\\..\\", string.Format(three, suffixThumb));

            if (File.Exists(onePath))
                File.Delete(onePath);
            if (File.Exists(oneThumbPath))
                File.Delete(oneThumbPath);

            if (File.Exists(twoPath))
                File.Delete(twoPath); 
            if (File.Exists(twoThumbPath))
                File.Delete(twoThumbPath);

            if (File.Exists(threePath))
                File.Delete(threePath);
            if (File.Exists(threeThumbPath))
                File.Delete(threeThumbPath);
        }

        [Fact]
        public void LargeImagesWrapWithLink()
        {
            var input = "<img src=\"files/one.png\" alt=\"smth\" title=\"else\">" +
                        "<img src=\"files/two.jpg\" alt=\"other\" title=\"stuff\">";

            var result = input.ProcessLargeImages(Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\"));
             
            var expected =
                "<img src=\"files/one-cmp.jpg\" alt=\"smth\" title=\"else\" width='272' height='260'>" +
                "<a href='files/two-cmp.jpg' class='without-caption image-link'>" +
                "   <img src=\"files/two-cmp-thumb.jpg\" alt=\"other\" title=\"stuff\" width='600' height='337'>" +
                "</a>";
            var res =
                "<img src=\"files/one-cmp.png\" alt=\"smth\" title=\"else\" width='272' height='260'>" +
                "<a href='files/two-cmp.jpg' class='without-caption image-link'>" +
                "   <img src=\"files/two-cmp-thumb.jpg\" alt=\"other\" title=\"stuff\" width='600' height='337'>" +
                "</a>";

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ImageResizerWorksForPng()
        {
            var result = ImageUtilities.GenerateVersions(onePathOrig, suffixOrig, suffixThumb);
            Assert.Equal(new List<string> { @"..\..\files/one-cmp-thumb.jpg", @"..\..\files/one-cmp.jpg" }, result);

        }

        [Fact]
        public void ImageResizerWorksForLargePng()
        {
            var result = ImageUtilities.GenerateVersions(twoPathOrig, suffixOrig, suffixThumb);
            Assert.Equal(new List<string> { @"..\..\files/two-cmp-thumb.jpg", @"..\..\files/two-cmp.jpg" }, result);

        }

        [Fact]
        public void ImageResizerWorksForJpg()
        {
            var result = ImageUtilities.GenerateVersions(threePathOrig, suffixOrig, suffixThumb);
            Assert.Equal(new List<string> { @"..\..\files/three-cmp-thumb.jpg", @"..\..\files/three-cmp.jpg" }, result);

        }
    }
}
