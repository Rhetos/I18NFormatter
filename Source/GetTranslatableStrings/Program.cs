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

            new Program() { Parameters = new Parameters(args) }.Run();
        }

        Parameters Parameters;

        void Run()
        {
            var files = Directory.GetFiles(Parameters.Root, "*.cs", SearchOption.AllDirectories)
                .Where(path => !Parameters.Exclude.Any(exclude => path.Contains(exclude)))
                .ToList();

            LogInfo("Reading " + files.Count() + " files.");

            var translatables = files.SelectMany(file =>
                {
                    LogTrace(() => "Parsing " + file + " ...");
                    var strings = new Parser(File.ReadAllText(file, Encoding.Default), Parameters).GetStrings();
                    foreach (var s in strings)
                        s.File = file;

                    int totalCount = strings.Count();
                    int translatableCount = strings.Count(s => s.Error == null);
                    if (totalCount > 0)
                        LogTrace(() => " Found " + translatableCount + " translatable and " + (totalCount - translatableCount) + " untranslatable messages.");

                    return strings;
                }).ToList();

            {
                int totalCount = translatables.Count();
                int translatableCount = translatables.Count(s => s.Error == null);
                if (totalCount > 0)
                    LogInfo("Found " + translatableCount + " translatable and " + (totalCount - translatableCount) + " untranslatable messages.");
            }

            string untranslatable = string.Join("\r\n", translatables
                .Where(t => t.Error != null)
                .Select(t =>
                    "\r\n#. " + t.Error
                    + "\r\n#. " + ReportFilePosition(t)
                    + "\r\n#. " + t.Context.Replace("\r", "").Replace("\n", "\r\n#. ")));

            string pot = string.Join("\r\n", translatables
                .Where(t => t.Error == null)
                .GroupBy(t => t.Text)
                .Select(g => new { Text = g.Key, Occurrences = g.OrderBy(t => t.File).ThenBy(t => t.Line).ToList() })
                .OrderBy(t => t.Occurrences.First().File).ThenBy(t => t.Occurrences.First().Line)
                .Select(t =>
                    string.Concat(t.Occurrences.Select(o => "\r\n#: " + ReportFilePosition(o)))
                    + "\r\nmsgid " + t.Text
                    + "\r\nmsgstr \"\""));

            if (!string.IsNullOrWhiteSpace(untranslatable))
                if (Parameters.IncludeUntranslatable)
                    pot = untranslatable + "\r\n" + pot;
                else
                    LogInfo(untranslatable);

            if (Parameters.PotFile != null)
                File.WriteAllText(Parameters.PotFile, pot + "\r\n", Encoding.UTF8);
            else
                Console.WriteLine(pot);
        }

        private string ReportFilePosition(TranslatableString t)
        {
            return t.File.Substring(Parameters.Root.Length + 1) + ":" + t.Line;
        }

        private void LogTrace(Func<string> msg)
        {
            if (Parameters.VerboseLog)
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
