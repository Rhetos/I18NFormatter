using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetTranslatableStrings
{
    public class Parameters
    {
        public readonly string Root = null;
        public readonly List<string> Exclude = new List<string>();
        public readonly bool VerboseLog = false;
        public readonly bool IncludeUntranslatable = false;
        /// <summary>Optional.</summary>
        public readonly string PotFile = null;

        public Parameters(string[] args)
        {
            const string excludeSwitch = "-exclude";
            const string verboseSwitch = "-verbose";
            const string potFileSwitch = "-pot";
            const string includeUntranslatableSwitch = "-includeUntranslatable";
            const string info = "Usage:\r\nGetTranslatableStrings.exe <source folder>"
                + " [" + excludeSwitch + " <path substring> ...]"
                + " [" + potFileSwitch + " <output file>]"
                + " [" + verboseSwitch + "]"
                + " [" + includeUntranslatableSwitch + "]"
                + "\r\n"
                + "\r\nOptions:"
                + "\r\n " + excludeSwitch + "  Exclude paths containing the substring. May be used multiple times."
                + "\r\n " + potFileSwitch + "  Output translation template to file. Standard output is used if undefined."
                + "\r\n " + verboseSwitch + "  Detailed logging."
                + "\r\n " + includeUntranslatableSwitch + "  Include untranslatable messages warnings in output."
                + "\r\n"
                + "\r\nExample:"
                + "\r\nGetTranslatableStrings.exe \"C:\\My Projects\\Rhetos\" " + excludeSwitch + " \\bin\\ " + excludeSwitch + " \\obj\\ -pot Rhetos.pot";
            if (args.Count() == 0)
                throw new ApplicationException("Missing parameters.\r\n\r\n" + info);

            var nextArgument = NextArgument.Undefined;
            foreach (var arg in args)
            {
                if (nextArgument == NextArgument.Exclude)
                {
                    Exclude.Add(arg);
                    nextArgument = NextArgument.Undefined;
                }
                else if (nextArgument == NextArgument.PotFile)
                {
                    PotFile = arg;
                    nextArgument = NextArgument.Undefined;
                }
                else
                {
                    if (nextArgument != NextArgument.Undefined)
                        throw new ApplicationException("Internal error: Unexpected argument type '" + nextArgument + "'.\r\n\r\n" + info);

                    if (!arg.StartsWith("-"))
                    {
                        if (Root == null)
                            Root = Path.GetFullPath(arg);
                        else
                            throw new ApplicationException("Unexpected parameter '" + arg + "'.\r\n\r\n" + info);
                    }
                    else if (arg.Equals(excludeSwitch, StringComparison.InvariantCultureIgnoreCase))
                        nextArgument = NextArgument.Exclude;
                    else if (arg.Equals(verboseSwitch, StringComparison.InvariantCultureIgnoreCase))
                        VerboseLog = true;
                    else if (arg.Equals(includeUntranslatableSwitch, StringComparison.InvariantCultureIgnoreCase))
                        IncludeUntranslatable = true;
                    else if (arg.Equals(potFileSwitch, StringComparison.InvariantCultureIgnoreCase))
                        nextArgument = NextArgument.PotFile;
                    else
                        throw new ApplicationException("Unexpected parameter '" + arg + "'.\r\n\r\n" + info);
                }
            }

            if (nextArgument == NextArgument.Exclude)
                throw new ApplicationException("Missing path parameter after '" + excludeSwitch + "'.\r\n\r\n" + info);

            if (nextArgument == NextArgument.PotFile)
                throw new ApplicationException("Missing file name after '" + potFileSwitch + "'.\r\n\r\n" + info);

            if (Root == null)
                throw new ApplicationException("Missing source folder parameter.\r\n\r\n" + info);

            if (VerboseLog)
            {
                Console.Error.WriteLine("Root: " + Root);
                for (int i = 0; i < Exclude.Count; i++)
                    Console.Error.WriteLine("Exclude " + (i + 1) + ": *" + Exclude[i] + "*");
                Console.Error.WriteLine(".pot file: " + (PotFile ?? "using stanard output"));
            }
        }
        enum NextArgument { Undefined, Exclude, PotFile };
    }
}
