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

namespace Rhetos.I18NFormatter
{
    /// <summary>
    /// Rhetos.I18NFormatter run-time configuration.
    /// </summary>
    [Options("Rhetos:I18NFormatter")]
    public class I18NFormatterOptions
    {
        /// <summary>
        /// When formatting message for localization, adds '(((' and ')))' to the parameters that are also intended for localization.
        /// For example, property and entity name in a user error message.
        /// See https://github.com/turquoiseowl/i18n/blob/master/README.md for more info.
        /// </summary>
        /// <remarks>
        /// Disabled by default to avoid breaking backward compatibility.
        /// </remarks>
        public bool LocalizeParameters { get; set; } = false;

        /// <summary>
        /// When formatting message for localization, adds '///' with a message context.
        /// See https://github.com/turquoiseowl/i18n/blob/master/README.md for more info.
        /// </summary>
        /// <remarks>
        /// Disabled by default to avoid breaking backward compatibility.
        /// </remarks>
        public bool AddMessageContext { get; set; } = false;
    }
}
