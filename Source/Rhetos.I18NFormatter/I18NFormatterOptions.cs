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
