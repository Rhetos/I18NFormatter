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
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GetTranslatableStrings.Test
{
    [TestClass]
    public class ParserTest
    {
        List<TranslatableString> Parse(string code)
        {
            var testParameters = new Parameters(new[] { "." });
            var testParser = new Parser(code, testParameters);
            return testParser.GetStrings();
        }

        [TestMethod]
        public void ParseEmpty()
        {
            Assert.AreEqual(0, Parse("").Count);
        }

        [TestMethod]
        public void ParseSimple()
        {
            var translatable = Parse("int main()\n{ _localizer[\"abc\"]; }").Single();
            Assert.AreEqual("_localizer[\"abc\"]", translatable.Context);
            Assert.AreEqual(2, translatable.Line);
            Assert.AreEqual(3, translatable.Column);
            Assert.AreEqual("abc", translatable.Text);
        }

        [TestMethod]
        public void ParseExcapes()
        {
            var tests = new Dictionary<string, string>
            {
                { @"_localizer[""1\t2""]", "1\t2" },
                { @"_localizer[@""1\t2""]", "1\\t2" },
            };

            string source = "int main() { "
                + string.Concat(tests.Keys.Select(input => input + "; "))
                + "}";

            var translatables = Parse(source);

            Assert.AreEqual(
                string.Join("|", tests.Values),
                string.Join("|", translatables.Select(t => t.Text)));
        }

        [TestMethod]
        public void ParseAlreadyInternationalized()
        {
            var t = Parse("int main()\n{ string localized = \"a\"; new UserException(localized); }").Single();
            Assert.AreEqual("Ignored, already internationalized.", t.Error);

            t = Parse("int main()\n{ new UserException(\"[[[abc]]]\"); }").Single();
            Assert.AreEqual("Ignored, already internationalized.", t.Error);

            t = Parse("int main()\n{ _localizer[\"[[[abc]]]\"]; }").Single();
            Assert.AreEqual("Ignored, already internationalized.", t.Error);
        }

        [TestMethod]
        public void ParseIgnoreTests()
        {
            var t = Parse("int main()\n{ new UserException(\"some [Test] code\"); }").Single();
            Assert.AreEqual("Ignored test.", t.Error);
        }
    }
}
