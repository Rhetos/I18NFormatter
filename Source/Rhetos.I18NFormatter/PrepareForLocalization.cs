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

using Rhetos.Utilities;
using System.ComponentModel.Composition;
using System.Text;
using System.Text.RegularExpressions;

namespace Rhetos.I18NFormatter
{
    /// <summary>
    /// This class actually does not translate the message to the end user's language.
    /// It prepares the messages format, to allow later localization by i18n ASP.NET plugin (https://github.com/turquoiseowl/i18n).
    /// </summary>
    [Export(typeof(ILocalizer))]
    public class PrepareForLocalization : ILocalizer
    {
        private static readonly Regex _tagsRegex = new Regex(@"{(\d+)}");

        public string this[object message, params object[] args]
        {
            get
            {
                string text = message.ToString();

                // Check if the message is already formatted:
                if (text.Contains("]]]"))
                    return text;

                // Convert string.Format parameters convention "{0}" with i18n convention "%0":
                text = _tagsRegex.Replace(text, "%$1");

                // Append message parameters:
                var parameters = new StringBuilder();
                if (args != null && args.Length > 0)
                    foreach (object arg in args)
                        parameters.Append("|||").Append(arg);

                return "[[[" + text + parameters.ToString() + "]]]";
            }
        }
    }
}
