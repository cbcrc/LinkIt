// Copyright (c) 2016 AgileObjects Ltd
// Licensed under the MIT license. See LICENSE file in the ReadableExpressions directory for more information.

namespace LinkIt.ReadableExpressions.Translators
{
    using System.Linq.Expressions;

    internal class ExtensionExpressionTranslator : ExpressionTranslatorBase
    {
        public ExtensionExpressionTranslator()
            : base(ExpressionType.Extension)
        {
        }

        public override string Translate(Expression expression, TranslationContext context)
        {
            return expression.ToString();
        }
    }
}