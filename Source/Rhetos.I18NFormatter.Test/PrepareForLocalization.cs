using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhetos.Utilities;

namespace Rhetos.I18NFormatter.Test
{
    [TestClass]
    public class PrepareForLocalizationTest
    {
        [TestMethod]
        public void Simple()
        {
            var localizer = new PrepareForLocalization();

            Assert.AreEqual("[[[a%0b%1|||00|||11]]]", localizer["a{0}b{1}", "00", 11]);
        }

        [TestMethod]
        public void NullArgs()
        {
            var localizer = new PrepareForLocalization();

            Assert.AreEqual("[[[a%0b|||]]]", localizer["a{0}b", new object[] { null }]);
            Assert.AreEqual("[[[a%0b%1|||0|||]]]", localizer["a{0}b{1}", 0, null]);
            Assert.AreEqual("[[[a%0b%1||||||]]]", localizer["a{0}b{1}", null, null]);
        }

        [TestMethod]
        public void NoArgs()
        {
            var localizer = new PrepareForLocalization();


            object[] zeroArgs = new object[] { };
            object[] noArgs = null;

            Assert.AreEqual("[[[ab]]]", localizer["ab", zeroArgs]);
            Assert.AreEqual("[[[ab]]]", localizer["ab", noArgs]);
        }
    }
}
