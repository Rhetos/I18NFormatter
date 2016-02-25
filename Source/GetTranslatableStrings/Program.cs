/*
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetTranslatableStrings
{
    class Program
    {
        static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += GlobalErrorHandler;

            new Program() { _parameters = new Parameters(args) }.Run();
        }

        private Parameters _parameters;

        void Run()
        {
            var files = _parameters.Folders
                .SelectMany(folder =>
                    Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories)
                    .Where(file => !_parameters.ExcludeSubstring.Any(exclude => file.Contains(exclude)))
                    .Select(file => new { RootFolder = folder, FileRelativePath = file.Substring(folder.Length + 1) }))
                .Concat(_parameters.Files.Select(file =>
                    new { RootFolder = Path.GetDirectoryName(file), FileRelativePath = Path.GetFileName(file) }))
                .ToList();

            LogInfo("Reading " + files.Count() + " files.");

            var translatables = files.SelectMany(file =>
                {
                    string filePath = Path.Combine(file.RootFolder, file.FileRelativePath);
                    LogTrace(() => "Parsing " + filePath + " ...");
                    string code = File.ReadAllText(filePath, Encoding.Default);
                    var strings = new Parser(code, _parameters).GetStrings();

                    foreach (var s in strings)
                    {
                        s.RootFolder = file.RootFolder;
                        s.FileRelativePath = file.FileRelativePath;
                    }

                    int totalCount = strings.Count();
                    int translatableCount = strings.Count(s => s.Error == null);
                    if (totalCount > 0)
                        LogTrace(() => "  Found " + translatableCount + " translatable and " + (totalCount - translatableCount) + " untranslatable messages in the file.");

                    return strings;
                }).ToList();
            LogTrace(() => ""); // Separator.

            {
                int totalCount = translatables.Count();
                int translatableCount = translatables.Count(s => s.Error == null);
                if (totalCount > 0)
                    LogInfo("Found " + translatableCount + " translatable and " + (totalCount - translatableCount) + " untranslatable messages.");
            }

            var pot = new Pot(_parameters);

            string untranslatable = pot.FormatErrors(translatables);
            string messages = pot.FormatMessages(translatables);

            if (!string.IsNullOrWhiteSpace(untranslatable))
                if (_parameters.IncludeUntranslatable)
                    messages = untranslatable + "\r\n" + messages;
                else
                    LogInfo(untranslatable);

            if (_parameters.PotFile != null)
                File.WriteAllText(_parameters.PotFile, messages + "\r\n", Encoding.UTF8);
            else
                Console.WriteLine(messages);
        }

        private void LogTrace(Func<string> msg)
        {
            if (_parameters.VerboseLog)
                Console.Error.WriteLine(msg());
        }

        private void LogInfo(string msg)
        {
            Console.Error.WriteLine(msg);
        }

        private static void GlobalErrorHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is ApplicationException)
                Console.WriteLine("ERROR: " + ((Exception)e.ExceptionObject).Message);
            else
                Console.WriteLine(e.ExceptionObject.ToString());
            Environment.Exit(1);
        }
    }
}
