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

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rhetos.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetTranslatableStrings
{
    public class Parser
    {
        private readonly string _code;
        private readonly Parameters _parameters;
        private Stopwatch _stopwatch;

        public Parser(string code, Parameters parameters)
        {
            _code = code;
            _parameters = parameters;
            _stopwatch = Stopwatch.StartNew();
        }

        public List<TranslatableString> GetStrings()
        {
            var strings = new List<TranslatableString>();
            var tree = CSharpSyntaxTree.ParseText(_code);

            foreach (SyntaxNode child in tree.GetRoot().ChildNodes())
                WalkSyntaxTree(strings, child);

            return strings;
        }

        private void WalkSyntaxTree(List<TranslatableString> strings, SyntaxNode node, int level = 0)
        {
            if (_parameters.VerboseLog && _stopwatch.ElapsedMilliseconds >= 4000)
            {
                Console.Error.WriteLine("Progress " + (node.SpanStart * 100 / _code.Length).ToString() + "%, " + node.SpanStart + "/" + _code.Length + ".");
                _stopwatch.Restart();
            }

            var text = TryGetString(node);
            if (text != null)
            {
                var translatable = NewTranslatableString(node);
                if (!text.IsError)
                    translatable.Text = text.Value;
                else
                    translatable.Error = text.Error;
                strings.Add(translatable);
            }

            foreach (SyntaxNode child in node.ChildNodes())
                WalkSyntaxTree(strings, child, level + 1);
        }

        private readonly string[] names = { "localizer", "_localizer" };

        private static ValueOrError<string> TryGetString(SyntaxNode node)
        {
            return
                TryGetStringParameter(node, new[] { "localizer", "_localizer" },
                    SyntaxKind.ElementAccessExpression, SyntaxKind.BracketedArgumentList)
                ?? TryGetStringParameter(node, new[] { "Rhetos.UserException", "UserException" },
                    SyntaxKind.ObjectCreationExpression, SyntaxKind.ArgumentList);
        }

        private static ValueOrError<string> TryGetStringParameter(SyntaxNode node, string[] names, SyntaxKind kind, SyntaxKind childKind)
        {
            if (node.Kind() == kind)
            {
                var childNodes = node.ChildNodes().ToList();

                if (childNodes.Count >= 2
                    && (childNodes[0].Kind() == SyntaxKind.IdentifierName || childNodes[0].Kind() == SyntaxKind.QualifiedName)
                    && names.Contains(childNodes[0].ToString()))
                {
                    if (childNodes[1].Kind() != childKind)
                        return ValueOrError.CreateError("Unexpected argument '" + childNodes[1].Kind() + "', expecting " + childKind + ".");

                    return TryGetStringParameter(childNodes[1].ChildNodes());
                }
            }
            return null;
        }

        private static ValueOrError<string> TryGetStringParameter(IEnumerable<SyntaxNode> messageArguments)
        {
            var firstArgument = messageArguments.FirstOrDefault();
            if (firstArgument == null)
                return ValueOrError.CreateError("Unexpected usage without arguments.");
            if (firstArgument.Kind() != SyntaxKind.Argument)
                return ValueOrError.CreateError("Unexpected child argument '" + firstArgument.Kind() + "', expecting " + SyntaxKind.Argument + ".");

            var firstArgumentExpression = firstArgument.ChildNodes().FirstOrDefault();
            if (firstArgumentExpression == null)
                return ValueOrError.CreateError("Unexpected child without arguments.");
            if (new[] { SyntaxKind.NullLiteralExpression, SyntaxKind.NumericLiteralExpression }.Contains(firstArgumentExpression.Kind()))
                return ValueOrError.CreateError("Ignored " + firstArgumentExpression.Kind() + ".");
            if (firstArgumentExpression.Kind() != SyntaxKind.StringLiteralExpression)
                return ValueOrError.CreateError("Unsupported argument " + firstArgumentExpression.Kind() + ".");

            var firstArgumentToken = firstArgumentExpression.ChildTokens().Single();
            return firstArgumentToken.ValueText;
        }

        private static TranslatableString NewTranslatableString(SyntaxNode node)
        {
            var position = node.SyntaxTree.GetLineSpan(node.Span).StartLinePosition;
            return new TranslatableString
            {
                Line = position.Line + 1,
                Column = position.Character + 1,
                Context = node.ToString()
            };
        }
    }
}
