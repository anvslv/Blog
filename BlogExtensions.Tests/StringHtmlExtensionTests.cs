// https://github.com/robvolk/Helpers.Net

using BlogExtensions.Extensions;
using Xunit; 

namespace Tests
{
    public class FineNameExtensionsTests
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

    /// <summary>
    /// Summary description for StringHtmlExtensionTests
    /// </summary> 
    public class StringHtmlExtensionTests
    {
        public StringHtmlExtensionTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        //public TestContext TestContext
        //{
        //    get
        //    {
        //        return testContextInstance;
        //    }
        //    set
        //    {
        //        testContextInstance = value;
        //    }
        //}

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        //[Fact]
        //public void ToDelimitedStringTest()
        //{
        //    Assert.Equal("", (new string[] { }).ToDelimitedString(","));
        //    Assert.Equal("", ((string[])null).ToDelimitedString(","));
        //    Assert.Equal("one", (new string[] { "one" }).ToDelimitedString(", "));
        //    Assert.Equal("one, two", (new string[] { "one", "two" }).ToDelimitedString(", "));
        //    Assert.Equal("one,two", (new string[] { "one", "two" }).ToDelimitedString(","));
        //}

        [Fact]
        public void StripHtmlTest()
        {
            Assert.Null(((string)null).StripHtml());

            Assert.Equal("hello", "hello".StripHtml());

            Assert.Equal("hello", "he<b>ll</b>o".StripHtml());
        }

        [Fact]
        public void TruncateTextTest()
        {
            Assert.Null(((string)null).StripHtml());

            string test = "1234567890";
            Assert.Equal("12345", test.Truncate(5, null));
            Assert.Equal("12345...", test.Truncate(5, "..."));
            Assert.Equal(string.Empty, string.Empty.Truncate(5, null));
            Assert.Equal("12", "12".Truncate(5));
        }

        [Fact]
        public void TruncateHtmlEmptyTest()
        {
            Assert.Null(((string)null).TruncateHtml(100));
            Assert.Equal(string.Empty.TruncateHtml(100), string.Empty);
        }

        [Fact]
        public void TruncateHtmlTextTest()
        {
            // no html test
            Assert.Equal("abc".TruncateHtml(10), "abc");
            Assert.Equal("abc".TruncateHtml(2), "ab");
        }

        [Fact]
        public void TruncateHtmlTest()
        {
            var html = @"<p>aaa <b>bbb</b>
ccc<br> ddd</p>";

            Assert.Equal(@"<p>aaa <b>bbb</b>
ccc<br> ddd</p>", html.TruncateHtml(100, "no trailing text")); // it ignores unclosed tags

            Assert.Equal(@"<p>aaa <b>bbb</b>
ccc<br>...</br></p>", html.TruncateHtml(14, "..."));

            Assert.Equal(@"<p>aaa <b>bbb</b></p>", html.TruncateHtml(10));

            // self closing test
            html = @"<p>hello<br/>there</p>";
            Assert.Equal(@"<p>hello<br/>th</p>", html.TruncateHtml(7));

            Assert.Equal("<b>i'm</b>", "<b>i'm awesome</b>".TruncateHtml(8));
            Assert.Equal("<b>i'm...</b>", "<b>i'm awesome</b>".TruncateHtml(8, "..."));
        }

        [Fact]
        public void TruncateWordsTest()
        {
            Assert.Null(((string)null).TruncateWords(100));
            Assert.Equal(string.Empty, string.Empty.TruncateWords(100));

            Assert.Equal("big brown", "big brown beaver".TruncateWords(12));
            Assert.Equal("big...", "big brown beaver".TruncateWords(5, "..."));
        }

        [Fact]
        public void TruncateWordsBreakingHtmlTagTest()
        {
            // truncates in the middle of a tag
            Assert.Equal("<b>i'm", "<b>i'm awesome</b>".TruncateWords(16));
        }
    }
}