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
using System.Text;
using System.Threading.Tasks;

namespace GetTranslatableStrings
{
    /// <summary>
    /// Helpers for generating a .pot file.
    /// </summary>
    public class Pot
    {
        private readonly Parameters _parameters;

        public Pot(Parameters parameters)
        {
            _parameters = parameters;
        }

        public string FormatErrors(IEnumerable<TranslatableString> translatables)
        {
            return string.Join("\r\n", translatables
                .Where(t => t.Error != null)
                .GroupBy(t => new { t.Error, t.Context })
                .Select(group => new { group.Key.Error, group.Key.Context, Occurrences = group.OrderBy(t => t, _sortByFileAndLine).ToList() })
                .OrderBy(t => t.Occurrences.First(), _sortByFileAndLine)
                .Select(t =>
                    "\r\n#. " + t.Error
                    + "\r\n#. " + MultilineComment(t.Context)
                    + FormatFilePositions(t.Occurrences)));
        }

        public string FormatMessages(IEnumerable<TranslatableString> translatables)
        {
            return string.Join("\r\n", translatables
                .Where(t => t.Error == null)
                .GroupBy(t => t.Text)
                .Select(group => new { Text = group.Key, Occurrences = group.OrderBy(t => t, _sortByFileAndLine).ToList() })
                .OrderBy(t => t.Occurrences.First(), _sortByFileAndLine)
                .Select(t =>
                    FormatFilePositions(t.Occurrences)
                    + "\r\nmsgid " + QuoteEscapeString(t.Text)
                    + "\r\nmsgstr \"\""));
        }

        private string FormatFilePositions(IList<TranslatableString> occurrences)
        {
            string multipleOccurrencesWarning = "";
            if (occurrences.Count() > 1)
                multipleOccurrencesWarning = "\r\n#. " + occurrences.Count() + " occurrences.";

            var occurrencePerFile = occurrences.GroupBy(o => o.FileRelativePath).Select(og => og.OrderBy(t => t, _sortByFileAndLine).First());
            return multipleOccurrencesWarning + string.Concat(occurrencePerFile.Select(o => "\r\n#: " + ReportFilePosition(o)));
        }

        static readonly IComparer<TranslatableString> _sortByFileAndLine = new SortByFileAndLine();

        class SortByFileAndLine : IComparer<TranslatableString>
        {
            public int Compare(TranslatableString a, TranslatableString b)
            {
                int diff = string.Compare(a.RootFolder, b.RootFolder, StringComparison.OrdinalIgnoreCase);
                if (diff != 0)
                    return diff;

                diff = string.Compare(a.FileRelativePath, b.FileRelativePath, StringComparison.OrdinalIgnoreCase);
                if (diff != 0)
                    return diff;

                return a.Line - b.Line;
            }
        }

        private static string MultilineComment(string comment)
        {
            return comment.Replace("\r", "").Replace("\n", "\r\n#. ");
        }

        public static string QuoteEscapeString(string text)
        {
            text = text.Replace("\\", "\\\\")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace("\t", "\\t")
                .Replace("\"", "\\\"");

            for (int i = 0; i < text.Length; i++)
                if (text[i] < 32)
                    throw new ApplicationException("Unexpected character (code " + ((int)text[i])
                        + ") in message '" + text + "'");

            return "\"" + text + "\"";
        }

        private string ReportFilePosition(TranslatableString t)
        {
            return t.FileRelativePath + ":" + t.Line;
        }
    }
}
