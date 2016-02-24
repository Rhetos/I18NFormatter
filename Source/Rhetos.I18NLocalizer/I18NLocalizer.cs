﻿/*
    Copyright (C) 2016 Omega software d.o.o.

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
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rhetos.I18NLocalizer
{
    /// <summary>
    /// This class actually does not translate the message to the end user's language.
    /// It prepares the messages format, to allow later localization by i18n ASP.NET plugin(https://github.com/turquoiseowl/i18n).
    /// </summary>
    [Export(typeof(ILocalizer))]
    public class I18NLocalizer : ILocalizer
    {
        public string this[object message, params object[] args]
        {
            get
            {
                string text = message.ToString();

                // Check if the message is already formatted:
                if (text.StartsWith("[[[") && text.EndsWith("]]]"))
                    return text;

                // Convert string.Format parameters convention "{0}" with i18n convention "%0":
                var tagsRegex = new Regex(@"{(\d+)}");
                text = tagsRegex.Replace(text, "%$1");

                // Evaluate parameters:
                var parameters = new StringBuilder();
                if (args != null && args.Length > 0)
                    foreach (object arg in args)
                        parameters.Append("|||").Append(arg.ToString());

                return "[[[" + text + parameters.ToString() + "]]]";
            }
        }
    }
}
