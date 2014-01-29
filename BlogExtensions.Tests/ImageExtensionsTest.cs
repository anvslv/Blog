using System.Collections.Generic;
using System.IO;
using BlogExtensions.Extensions;
using Xunit;

namespace Tests
{ 
    public class ImageExtensionsTests
    {
        private const string directory = ".\\"; 
        private string one = "files/one{0}.png";
        private string two = "files/two{0}.jpg";
        private string three = "files/three{0}.png"; 

        private const string suffixOrig = "-cmp";
        private const string suffixThumb = "-cmp-thumb";

        string onePathOrig = "";
        string twoPathOrig = "";
        string threePathOrig = "";

        string onePath = "";
        string oneThumbPath = "";
        string twoPath = "";
        string twoThumbPath = "";
        string threePath = "";
        string threeThumbPath = "";

        public ImageExtensionsTests()
        { 
            onePathOrig = Path.Combine(directory, string.Format(one, ""));
            twoPathOrig = Path.Combine(directory, string.Format(two, ""));
            threePathOrig = Path.Combine(directory, string.Format(three, ""));

            onePath = Path.Combine(directory, string.Format(one, "").AddSuffixAndReplaceExtension(suffixOrig));
            oneThumbPath = Path.Combine(directory, string.Format(one, "").AddSuffixAndReplaceExtension(suffixThumb));

            twoPath = Path.Combine(directory, string.Format(two, "").AddSuffixAndReplaceExtension(suffixOrig));
            twoThumbPath = Path.Combine(directory, string.Format(two, "").AddSuffixAndReplaceExtension(suffixThumb));

            threePath = Path.Combine(directory, string.Format(three, "").AddSuffixAndReplaceExtension(suffixOrig));
            threeThumbPath = Path.Combine(directory, string.Format(three, "").AddSuffixAndReplaceExtension(suffixThumb));

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

            var result = input.ProcessLargeImages(Path.Combine(Directory.GetCurrentDirectory(), directory));
             
            var expected =
                "<img src=\"files/one-cmp.jpg\" alt=\"smth\" title=\"else\" width='272' height='260'>" +
                "<a href='files/two-cmp.jpg' class='without-caption image-link'>" +
                "   <img src=\"files/two-cmp-thumb.jpg\" alt=\"other\" title=\"stuff\" width='600' height='337'>" +
                "</a>"; 

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ImageResizerWorksForPng()
        {
            var result = ImageUtilities.GenerateVersions(onePathOrig, suffixOrig, suffixThumb);
            Assert.Equal(new List<string> { oneThumbPath, onePath }, result);

        }

        [Fact]
        public void ImageResizerWorksForLargePng()
        {
            var result = ImageUtilities.GenerateVersions(twoPathOrig, suffixOrig, suffixThumb);
            Assert.Equal(new List<string> { twoThumbPath, twoPath}, result);

        }

        [Fact]
        public void ImageResizerWorksForJpg()
        {
            var result = ImageUtilities.GenerateVersions(threePathOrig, suffixOrig, suffixThumb);
            Assert.Equal(new List<string> { threeThumbPath,  threePath}, result);

        }
    }
}
