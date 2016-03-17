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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetTranslatableStrings
{
    public class Parameters
    {
        public readonly List<string> Folders = new List<string>();
        public readonly List<string> Files = new List<string>();
        public readonly List<string> ExcludeSubstring = new List<string>();
        public readonly bool VerboseLog = false;
        public readonly bool IncludeUntranslatable = false;
        /// <summary>Optional.</summary>
        public readonly string PotFile = null;

        public Parameters(string[] args)
        {
            const string excludeSwitch = "-exclude";
            const string verboseSwitch = "-verboseLog";
            const string potFileSwitch = "-pot";
            const string includeUntranslatableSwitch = "-includeUntranslatable";
            const string info = "Usage:\r\nGetTranslatableStrings.exe <source folder or file ...>"
                + " [" + excludeSwitch + " <path substring> ...]"
                + " [" + potFileSwitch + " <output file>]"
                + " [" + verboseSwitch + "]"
                + " [" + includeUntranslatableSwitch + "]"
                + "\r\n"
                + "\r\nOptions:"
                + "\r\n Multiple source folders or files may be provided."
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
                    ExcludeSubstring.Add(arg);
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

                    if (arg.Equals(excludeSwitch, StringComparison.InvariantCultureIgnoreCase))
                        nextArgument = NextArgument.Exclude;
                    else if (arg.Equals(verboseSwitch, StringComparison.InvariantCultureIgnoreCase))
                        VerboseLog = true;
                    else if (arg.Equals(includeUntranslatableSwitch, StringComparison.InvariantCultureIgnoreCase))
                        IncludeUntranslatable = true;
                    else if (arg.Equals(potFileSwitch, StringComparison.InvariantCultureIgnoreCase))
                        nextArgument = NextArgument.PotFile;
                    else if (Directory.Exists(arg))
                        Folders.Add(Path.GetFullPath(arg));
                    else if (arg.EndsWith(".cs") && File.Exists(arg))
                        Files.Add(Path.GetFullPath(arg));
                    else
                        throw new ApplicationException("Unexpected parameter '" + arg + "'. It is not a valid folder, C# file or option.\r\n\r\n" + info);
                }
            }

            if (nextArgument == NextArgument.Exclude)
                throw new ApplicationException("Missing path parameter after '" + excludeSwitch + "'.\r\n\r\n" + info);

            if (nextArgument == NextArgument.PotFile)
                throw new ApplicationException("Missing file name after '" + potFileSwitch + "'.\r\n\r\n" + info);

            if (Folders.Count() == 0 && Files.Count() == 0)
                throw new ApplicationException("Missing source folder or file parameter.\r\n\r\n" + info);

            if (VerboseLog)
            {
                Console.Error.WriteLine("Parameters:");
                for (int i = 0; i < Folders.Count; i++)
                    Console.Error.WriteLine("  Source folder " + (i + 1) + ": " + Folders[i]);
                for (int i = 0; i < Files.Count; i++)
                    Console.Error.WriteLine("  Source file " + (i + 1) + ": " + Files[i]);
                for (int i = 0; i < ExcludeSubstring.Count; i++)
                    Console.Error.WriteLine("  Exclude " + (i + 1) + ": *" + ExcludeSubstring[i] + "*");
                Console.Error.WriteLine("  POT file: " + (PotFile ?? "using stanard output"));
                Console.Error.WriteLine();
            }
        }
        enum NextArgument { Undefined, Exclude, PotFile };
    }
}
