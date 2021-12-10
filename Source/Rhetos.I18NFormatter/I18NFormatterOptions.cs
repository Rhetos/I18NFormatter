using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhetos.I18NFormatter
{
    [Options("Rhetos.I18NFormatter")]
    public class I18NFormatterOptions
    {
        /// <remarks>
        /// Disabled by default to avoid breaking backward compatibility.
        /// </remarks>
        public bool AddMessageContext { get; set; }
    }
}
