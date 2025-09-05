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

namespace GetTranslatableStrings.Test
{
    public class ParserTest
    {
        List<TranslatableString> Parse(string code)
        {
            var testParameters = new Parameters(new[] { "." });
            var testParser = new Parser(code, testParameters);
            return testParser.GetStrings();
        }

        [Fact]
        public void ParseEmpty()
        {
            Assert.Empty(Parse(""));
        }

        [Fact]
        public void ParseSimple()
        {
            var translatable = Parse("int main()\n{ _localizer[\"abc\"]; }").Single();
            Assert.Equal("_localizer[\"abc\"]", translatable.Context);
            Assert.Equal(2, translatable.Line);
            Assert.Equal(3, translatable.Column);
            Assert.Equal("abc", translatable.Text);
        }

        [Fact]
        public void ParseEscapes()
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

            Assert.Equal(
                string.Join("|", tests.Values),
                string.Join("|", translatables.Select(t => t.Text)));
        }

        [Fact]
        public void ParseAlreadyInternationalized()
        {
            var t = Parse("int main()\n{ string localized = \"a\"; new UserException(localized); }").Single();
            Assert.Equal("Ignored, already internationalized.", t.Error);

            t = Parse("int main()\n{ new UserException(\"[[[abc]]]\"); }").Single();
            Assert.Equal("Ignored, already internationalized.", t.Error);

            t = Parse("int main()\n{ _localizer[\"[[[abc]]]\"]; }").Single();
            Assert.Equal("Ignored, already internationalized.", t.Error);
        }

        [Fact]
        public void ParseIgnoreTests()
        {
            var t = Parse("int main()\n{ new UserException(\"some [Test] code\"); }").Single();
            Assert.Equal("Ignored test.", t.Error);
        }

        [Fact]
        public void CustomInvalidDataMessage()
        {
            var translatable = Parse("int main()\n{ new InvalidDataMessage { ID = Guid.NewGuid(), Message = \"Invalid {0} because {1}.\", MessageParameters = new object[] { 1, 2 } }; }").Single();
            Assert.Equal("Invalid {0} because {1}.", translatable.Text);
        }

    }
}
