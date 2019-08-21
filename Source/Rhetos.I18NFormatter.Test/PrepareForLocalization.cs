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
