/*
    Copyright (C) 2014 Omega software d.o.o.

    This file is part of Rhetos.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhetos.TestCommon;
using System;

namespace Rhetos.I18NFormatter.Test
{
    [TestClass]
    public class PrepareForLocalizationTest
    {
        [TestMethod]
        public void Simple()
        {
            var localizer = new PrepareForLocalization(new I18NFormatterOptions());

            Assert.AreEqual("[[[a%0b%1|||00|||11]]]", localizer["a{0}b{1}", "00", 11]);
        }

        [TestMethod]
        public void NullArgs()
        {
            var localizer = new PrepareForLocalization(new I18NFormatterOptions());

            Assert.AreEqual("[[[a%0b|||]]]", localizer["a{0}b", new object[] { null }]);
            Assert.AreEqual("[[[a%0b%1|||0|||]]]", localizer["a{0}b{1}", 0, null]);
            Assert.AreEqual("[[[a%0b%1||||||]]]", localizer["a{0}b{1}", null, null]);
        }

        [TestMethod]
        public void NoArgs()
        {
            var localizer = new PrepareForLocalization(new I18NFormatterOptions());

            object[] zeroArgs = new object[] { };
            object[] noArgs = null;

            Assert.AreEqual("[[[ab]]]", localizer["ab", zeroArgs]);
            Assert.AreEqual("[[[ab]]]", localizer["ab", noArgs]);
        }

        [TestMethod]
        public void LocalizedParameters()
        {
            var localizer = new PrepareForLocalization(new I18NFormatterOptions());

            string message = "User {0} should not change {1} in {2}.";

            object[] args = new object[]
            {
                "SomeUserName", // Some parameters should not be translated.
                localizer["SomeProperty"],
                localizer["SomeModule.SomeEntity"],
            };

            Assert.AreEqual(
                "[[[User %0 should not change %1 in %2." +
                    "|||SomeUserName" +
                    "|||(((SomeProperty)))" +
                    "|||(((SomeModule.SomeEntity)))" +
                    "]]]",
                localizer[message, args]);
        }

        [TestMethod]
        public void LocalizedParametersWithInnerArguments()
        {
            var localizer = new PrepareForLocalization(new I18NFormatterOptions());

            string message = "Message {0}";
            object[] args = new object[]
            {
                localizer["{0}{1}", 12, 34], // Localized message parameters with inner arguments are not supported.
            };

            TestUtility.ShouldFail<ArgumentException>(
                () => { _ = localizer[message, args]; },
                "Only simple localized message parameters are supported");
        }

        [TestMethod]
        public void MessageContextDisabled()
        {
            var localizer = new PrepareForLocalization<TestModule.TestEntity>(
                new I18NFormatterOptions { AddMessageContext = false });

            Assert.AreEqual("[[[ab]]]", localizer["ab"]);
        }

        [TestMethod]
        public void MessageContextEnabled()
        {
            var localizer = new PrepareForLocalization<TestModule.TestEntity>(
                new I18NFormatterOptions { AddMessageContext = true });

            Assert.AreEqual("[[[ab///TestModule.TestEntity]]]", localizer["ab"]);
        }
    }
}

namespace TestModule
{
    public class TestEntity
    {
    }
}
