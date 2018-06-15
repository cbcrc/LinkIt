// Copyright (c) 2016 AgileObjects Ltd
// Licensed under the MIT license. See LICENSE file in the ReadableExpressions directory for more information.

namespace LinkIt.ReadableExpressions.Translators
{
    using System.Linq;
    using System.Linq.Expressions;
    using Extensions;

    internal class NewArrayExpressionTranslator : ExpressionTranslatorBase
    {
        internal NewArrayExpressionTranslator()
            : base(ExpressionType.NewArrayBounds)
        {
        }

        public override string Translate(Expression expression, TranslationContext context)
        {
            var newArray = (NewArrayExpression)expression;

            var arrayTypeName = expression.Type.GetElementType().GetFriendlyName();

            var bounds = string.Join(
                "][",
                newArray.Expressions.Select(context.Translate));

            return $"new {arrayTypeName}[{bounds}]";
        }
    }
}