// Copyright (c) 2016 AgileObjects Ltd
// Licensed under the MIT license. See LICENSE file in the ReadableExpressions directory for more information.

namespace LinkIt.ReadableExpressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Extensions;
    using static System.Environment;

    internal static class StringExtensions
    {
        private static readonly char[] _terminatingCharacters = { ';', ':', ',' };

        public static bool IsTerminated(this string codeLine)
        {
            if (!codeLine.EndsWith('}'))
            {
                return codeLine.EndsWithAny(_terminatingCharacters) || codeLine.IsComment();
            }

            var lastNewLine = codeLine.LastIndexOf(NewLine, StringComparison.Ordinal);

            if (lastNewLine == -1)
            {
                return false;
            }

            var codeLines = codeLine.TrimStart().SplitToLines();

            while (codeLines[0].StartsWith(IndentSpaces, StringComparison.Ordinal))
            {
                for (var i = 0; i < codeLines.Length; i++)
                {
                    if (codeLines[i].Length != 0)
                    {
                        codeLines[i] = codeLines[i].Substring(IndentSpaces.Length);
                    }
                }
            }

            var lastNonIndentedCodeLine = codeLines
                .Last(line => line.IsNonIndentedCodeLine());

            if ((lastNonIndentedCodeLine == "catch") ||
                 lastNonIndentedCodeLine.StartsWith("if ", StringComparison.Ordinal) ||
                 lastNonIndentedCodeLine.StartsWith("while ", StringComparison.Ordinal) ||
                (lastNonIndentedCodeLine == "finally") ||
                 lastNonIndentedCodeLine.StartsWith("else if ", StringComparison.Ordinal) ||
                (lastNonIndentedCodeLine == "else") ||
                 lastNonIndentedCodeLine.StartsWith("switch ", StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        public static bool IsNonIndentedCodeLine(this string line)
        {
            return line.IsNotIndented() && !line.StartsWith('{') && !line.StartsWith('}');
        }

        public static string Unterminated(this string codeLine)
        {
            return codeLine.EndsWith(';')
                ? codeLine.Substring(0, codeLine.Length - 1)
                : codeLine;
        }

        private static readonly string[] _newLines = { NewLine };

        public static string[] SplitToLines(this string line, StringSplitOptions splitOptions = StringSplitOptions.None)
        {
            return line.Split(_newLines, splitOptions);
        }

        private const string IndentSpaces = "    ";

        public static bool IsNotIndented(this string line)
        {
            return (line.Length > 0) && !line.StartsWith(IndentSpaces, StringComparison.Ordinal);
        }

        public static string Indented(this string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return string.Empty;
            }

            if (line.IsMultiLine())
            {
                return string.Join(
                    NewLine,
                    line.SplitToLines().Select(l => l.Indented()));
            }

            return IndentSpaces + line;
        }

        public static bool IsMultiLine(this string value)
            => (value != NewLine) && value.Contains(NewLine);

        private const string UnindentPlaceholder = "*unindent*";

        public static string Unindented(this string line)
        {
            return UnindentPlaceholder + line;
        }

        public static string WithoutUnindents(this string code)
        {
            if (code.Contains(UnindentPlaceholder))
            {
                return code
                    .Replace(IndentSpaces + UnindentPlaceholder, null)
                    .Replace(UnindentPlaceholder, null);
            }

            return code;
        }

        public static string WithSurroundingParentheses(this string value)
        {
            if (!value.HasSurroundingParentheses())
            {
                return "(" + value + ")";
            }

            var openParenthesesCount = 0;

            for (var i = 1; i < value.Length - 1; ++i)
            {
                switch (value[i])
                {
                    case '(':
                        ++openParenthesesCount;
                        continue;

                    case ')':
                        --openParenthesesCount;

                        if (openParenthesesCount < 0)
                        {
                            return "(" + value + ")";
                        }

                        continue;
                }
            }

            return value;
        }

        public static string WithoutSurroundingParentheses(this string value, Expression expression)
        {
            if (string.IsNullOrEmpty(value) || KeepSurroundingParentheses(expression))
            {
                return value;
            }

            return value.HasSurroundingParentheses()
                ? value.Substring(1, value.Length - 2)
                : value;
        }

        private static bool HasSurroundingParentheses(this string value)
        {
            return value.StartsWith('(') && value.EndsWith(')');
        }

        private static bool KeepSurroundingParentheses(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Conditional:
                case ExpressionType.Lambda:
                    return true;

                case ExpressionType.Call:
                    var parentExpression = expression.GetParentOrNull();
                    while (parentExpression != null)
                    {
                        expression = parentExpression;
                        parentExpression = expression.GetParentOrNull();
                    }

                    switch (expression.NodeType)
                    {
                        case ExpressionType.Add:
                        case ExpressionType.Convert:
                        case ExpressionType.Multiply:
                        case ExpressionType.Subtract:
                            return true;
                    }

                    return false;

                case ExpressionType.Invoke:
                    var invocation = (InvocationExpression)expression;

                    return invocation.Expression.NodeType == ExpressionType.Lambda;
            }

            return false;
        }

        public static bool StartsWithNewLine(this string value)
        {
            return value.StartsWith(NewLine, StringComparison.Ordinal);
        }

        public static bool StartsWith(this string value, char character)
        {
            return (value.Length > 0) && (value[0] == character);
        }

        public static bool EndsWith(this string value, char character)
        {
            return (value != string.Empty) && value.EndsWithNoEmptyCheck(character);
        }

        private static bool EndsWithAny(this string value, IEnumerable<char> characters)
        {
            return (value != string.Empty) && characters.Any(value.EndsWithNoEmptyCheck);
        }

        private static bool EndsWithNoEmptyCheck(this string value, char character)
        {
            return value[value.Length - 1] == character;
        }

        private const string CommentString = "// ";

        public static string AsComment(this string text)
        {
            return CommentString + text
                .Trim()
                .Replace(NewLine, NewLine + CommentString);
        }

        public static bool IsComment(this string codeLine)
        {
            return codeLine.TrimStart().StartsWith(CommentString, StringComparison.Ordinal);
        }

        public static string ToStringConcatenation(this IEnumerable<Expression> strings, TranslationContext context)
        {
            return string.Join(" + ", strings.Select((str => GetStringValue(str, context))));
        }

        private static string GetStringValue(Expression value, TranslationContext context)
        {
            if (value.NodeType == ExpressionType.Call)
            {
                var methodCall = (MethodCallExpression)value;

                if ((methodCall.Method.Name == "ToString") && !methodCall.Arguments.Any())
                {
                    value = methodCall.GetSubject();
                }
            }

            var stringValue = context.Translate(value);

            switch (value.NodeType)
            {
                case ExpressionType.Conditional:
                    stringValue = stringValue.WithSurroundingParentheses();
                    break;
            }

            return stringValue;
        }
    }
}