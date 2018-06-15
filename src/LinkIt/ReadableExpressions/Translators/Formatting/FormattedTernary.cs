// Copyright (c) 2016 AgileObjects Ltd
// Licensed under the MIT license. See LICENSE file in the ReadableExpressions directory for more information.

namespace LinkIt.ReadableExpressions.Translators.Formatting
{
    using System;
    using System.Linq.Expressions;

    internal class FormattedTernary : FormattableExpressionBase
    {
        public FormattedTernary(
            Expression condition,
            CodeBlock ifTrue,
            CodeBlock ifFalse,
            TranslationContext context)
        {
            var test = GetTest(condition, context);
            var ifTrueString = GetBranch(ifTrue);
            var ifFalseString = GetBranch(ifFalse);

            SingleLineTranslationFactory = () => $"{test} ?{ifTrueString} :{ifFalseString}";

            MultipleLineTranslationFactory = () =>
                test +
                Environment.NewLine +
                ("?" + ifTrueString).Indented() +
                Environment.NewLine +
                (":" + ifFalseString).Indented();
        }

        private static string GetTest(Expression condition, TranslationContext context)
        {
            var test = context.Translate(condition);

            if ((condition.NodeType == ExpressionType.Call) ||
                test.IndexOf(" ", StringComparison.Ordinal) == -1)
            {
                return test.WithoutSurroundingParentheses(condition);
            }

            return test.WithSurroundingParentheses();
        }

        private static string GetBranch(CodeBlock codeBlock)
        {
            return codeBlock.IsASingleStatement
                ? codeBlock.AsExpressionBody()
                : " " + codeBlock.WithCurlyBraces().TrimStart();
        }

        protected override Func<string> SingleLineTranslationFactory { get; }

        protected override Func<string> MultipleLineTranslationFactory { get; }
    }
}