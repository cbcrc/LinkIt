// Copyright (c) 2016 AgileObjects Ltd
// Licensed under the MIT license. See LICENSE file in the ReadableExpressions directory for more information.

namespace LinkIt.ReadableExpressions.Translators
{
    using System.Linq.Expressions;

    internal class RuntimeVariablesExpressionTranslator : ExpressionTranslatorBase
    {
        public RuntimeVariablesExpressionTranslator()
            : base(ExpressionType.RuntimeVariables)
        {
        }

        public override string Translate(Expression expression, TranslationContext context)
        {
            var runtimeVariables = (RuntimeVariablesExpression)expression;

            var translated = context
                .TranslateParameters(runtimeVariables.Variables)
                .WithParenthesesIfNecessary();

            return translated;
        }
    }
}