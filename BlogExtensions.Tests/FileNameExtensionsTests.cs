using BlogExtensions.Extensions;
using Xunit;

namespace Blog.Extensions.Tests
{
    public class FileNameExtensionsTests
    {
        [Fact]
        public void SuffixIsAddedToPath()
        {
            var suffix = "-resized";
            var path = "c:\\dir\\text.txt";
            var expected = "c:\\dir\\text" + suffix + ".txt";

            Assert.Equal(expected, path.AddSuffix(suffix));
        }

        [Fact]
        public void SuffixIsAddedToUrl()
        {
            var suffix = "-resized";
            var path = "/this/path/is/relative/text.txt";
            var expected = "/this/path/is/relative/text" + suffix + ".txt";

            Assert.Equal(expected, path.AddSuffix(suffix));
        }

    }
}