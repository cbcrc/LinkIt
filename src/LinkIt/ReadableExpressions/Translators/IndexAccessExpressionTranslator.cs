// Copyright (c) 2016 AgileObjects Ltd
// Licensed under the MIT license. See LICENSE file in the ReadableExpressions directory for more information.

namespace LinkIt.ReadableExpressions.Translators
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class IndexAccessExpressionTranslator : ExpressionTranslatorBase
    {
        public IndexAccessExpressionTranslator()
            : base(ExpressionType.ArrayIndex, ExpressionType.Index)
        {
        }

        public override string Translate(Expression expression, TranslationContext context)
        {
            if (expression.NodeType == ExpressionType.Index)
            {
                return TranslateIndexedPropertyAccess(expression, context);
            }

            var arrayAccess = (BinaryExpression)expression;

            return TranslateIndexAccess(arrayAccess.Left, new[] { arrayAccess.Right }, context);
        }

        private string TranslateIndexedPropertyAccess(Expression expression, TranslationContext context)
        {
            var index = (IndexExpression)expression;

            return TranslateIndexAccess(index.Object, index.Arguments, context);
        }

        internal string TranslateIndexAccess(
            Expression variable,
            IEnumerable<Expression> indexes,
            TranslationContext context)
        {
            var indexedVariable = context.Translate(variable);
            var indexValues = context.TranslateParameters(indexes).WithoutParentheses();

            return $"{indexedVariable}[{indexValues}]";
        }
    }
}