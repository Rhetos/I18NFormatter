using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetTranslatableString
{
    class Program
    {
        static void Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += GlobalErrorHandler;

            var files = GetFiles(@"C:\My Projects\Rhetos", @"Source\Rhetos\bin"); // Core
            //var files = GetFiles(@"C:\My Projects\Rhetos\Source\Rhetos\bin", @"xxx"); // Generated
            //var files = new[] { @"C:\My Projects\Rhetos\Source\Rhetos.Utilities.Test\NoLocalizerTest.cs " };

            foreach (string path in files)
            {
                Console.WriteLine(path);
                var strings = Parse(File.ReadAllText(path, Encoding.Default));
            }
        }

        class TranslatableString
        {
            // Either Text or Error should be set:
            public string Text;
            public string Error;

            // Context:
            public string File;
            public int Line;
            public int Column;
            public string Context;
        }

        private static void GlobalErrorHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is ApplicationException)
                Console.WriteLine("ERROR: " + ((Exception)e.ExceptionObject).Message);
            else
                Console.WriteLine(e.ExceptionObject.ToString());
            Environment.Exit(1);
        }

        private static List<string> GetFiles(string root, string except)
        {
            root = Path.GetFullPath(root);
            except = Path.Combine(root, except);
            return Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
                .Where(path => !path.StartsWith(except))
                .Where(path => !path.Contains(@"\obj\"))
                .ToList();
        }

        private static List<TranslatableString> Parse(string code)
        {
            var strings = new List<TranslatableString>();
            var tree = CSharpSyntaxTree.ParseText(code);
            foreach (SyntaxNode child in tree.GetRoot().ChildNodes())
            {
                WalkSyntaxTree(strings, child);
            }
            return strings;
        }

        private static void WalkSyntaxTree(List<TranslatableString> strings, SyntaxNode node)
        {
            if (node != null)
            {
                if (node.Kind() == SyntaxKind.ElementAccessExpression)
                {
                    var childNodes = node.ChildNodes().ToList();
                    if (childNodes.Count >= 2
                        && childNodes[0].Kind() == SyntaxKind.IdentifierName
                        && new[] { "localizer", "_localizer" }.Contains(childNodes[0].ToString())
                        && childNodes[1].Kind() == SyntaxKind.BracketedArgumentList)
                    {
                        var translatable = new TranslatableString
                        {
                            File = null,
                            Line = 0,
                            Column = 0,
                            Context = null,
                        };

                        try
                        {
                            if (childNodes[1].Kind() != SyntaxKind.BracketedArgumentList)
                                throw new ApplicationException("childNodes[1].Kind() '" + childNodes[1].Kind().ToString() + "' != SyntaxKind.BracketedArgumentList");

                            var firstArgument = childNodes[1].ChildNodes().First();
                            if (firstArgument.Kind() != SyntaxKind.Argument)
                                throw new ApplicationException("firstArgument.Kind() '" + firstArgument.Kind().ToString() + "' != SyntaxKind.Argument");

                            var firstArgumentValue = firstArgument.ChildNodes().First();

                            Console.WriteLine(node.ToString());
                            Console.WriteLine("  " + firstArgumentValue.Kind() + " " + firstArgumentValue.ToString());

                            if (firstArgumentValue.Kind() == SyntaxKind.StringLiteralExpression)
                            {
                                translatable.Text = firstArgument.ToString();
                                Console.WriteLine("    STRING " + firstArgumentValue.Kind() + " " + firstArgumentValue.ToString());
                            }
                            else if (new[] { SyntaxKind.NullLiteralExpression, SyntaxKind.NumericLiteralExpression }.Contains(firstArgumentValue.Kind()))
                            {
                                Console.WriteLine("    IGNORE " + firstArgumentValue.Kind() + " " + firstArgumentValue.ToString());
                            }
                            else
                                throw new ApplicationException("    UNSUPPORTED " + firstArgumentValue.Kind() + " " + firstArgumentValue.ToString());

                            if (translatable.Text != null)
                                strings.Add(translatable);
                        }
                        catch (ApplicationException ex)
                        {
                            translatable.Error = ex.GetType().Name + ": " + ex.Message;
                        }
                    }
                }

                foreach (SyntaxNode child in node.ChildNodes())
                    WalkSyntaxTree(strings, child);
            }
        }
    }
}
