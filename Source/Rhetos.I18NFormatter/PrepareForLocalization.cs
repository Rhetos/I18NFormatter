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

using Microsoft.Extensions.Localization;
using Rhetos.Utilities;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Rhetos.I18NFormatter
{
    /// <summary>
    /// This class actually does not translate the message to the end user's language.
    /// It prepares the messages format, to allow later localization by i18n ASP.NET plugin (https://github.com/turquoiseowl/i18n).
    /// </summary>
    public class PrepareForLocalization : ILocalizer
    {
        private static readonly Regex _tagsRegex = new Regex(@"{(\d+)}");

        private readonly I18NFormatterOptions _options;
        private readonly string _messageContext;

        public PrepareForLocalization(I18NFormatterOptions options)
            : this(options, null)
        {
        }

        protected PrepareForLocalization(I18NFormatterOptions options, string messageContext)
        {
            _options = options;
            _messageContext = messageContext;
        }

        public LocalizedString this[object message, params object[] args]
        {
            get
            {
                return FormatForLocalization(message, args);
            }
        }

        private TokenizedString FormatForLocalization(object message, object[] args)
        {
            bool hasArguments = args != null && args.Length > 0;

            // Return the original message if it has been already formatted by localizer:
            if (message is TokenizedString tokenizedString && !hasArguments)
                return tokenizedString;

            string text = message.ToString();

            if (text.Contains("]]]") && !hasArguments)
                return new TokenizedString(text, text, hasArguments);

            var result = new StringBuilder();
            result.Append("[[[");

            // Convert string.Format parameters convention "{0}" with i18n convention "%0":
            result.Append(_tagsRegex.Replace(text, "%$1"));

            if (_options.AddMessageContext)
            {
                // Add the comment tag. It can be used for context-dependent localization with msgctxt in .po files.
                if (_messageContext != null)
                    result.Append("///").Append(_messageContext);
            }

            // Append message parameters:
            if (args != null)
                foreach (object arg in args)
                {
                    result.Append("|||");
                    if (arg is TokenizedString tokenizedArgument)
                    {
                        if (!tokenizedArgument.HadArguments)
                        {
                            if (_options.LocalizeParameters)
                                result.Append($"((({tokenizedArgument.Name})))"); // Special token syntax for localization of message parameters.
                            else
                                result.Append($"{tokenizedArgument.Name}"); // Parameter localization disabled for backward compatibility.
                        }
                        else
                            throw new ArgumentException("Only simple localized message parameters are supported: Localized message parameters cannot have additional inner parameters.");
                    }
                    else
                        result.Append(arg); // Exact parameter text that should not be localized.
                }

            result.Append("]]]");
            return new TokenizedString(text, result.ToString(), hasArguments);
        }
    }

    /// <summary>
    /// This class actually does not translate the message to the end user's language.
    /// It prepares the messages format, to allow later localization by i18n ASP.NET plugin (https://github.com/turquoiseowl/i18n).
    /// </summary>
    public class PrepareForLocalization<T> : PrepareForLocalization, ILocalizer<T>
    {
        public PrepareForLocalization(I18NFormatterOptions options)
            : base(options, typeof(T).FullName)
        {
        }
    }
}
