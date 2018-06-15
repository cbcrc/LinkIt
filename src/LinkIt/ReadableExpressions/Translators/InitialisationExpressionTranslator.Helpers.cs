﻿// Copyright (c) 2016 AgileObjects Ltd
// Licensed under the MIT license. See LICENSE file in the ReadableExpressions directory for more information.

namespace LinkIt.ReadableExpressions.Translators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal partial class InitialisationExpressionTranslator
    {
        private interface IInitExpressionHelper
        {
            string Translate(Expression expression, TranslationContext context);
        }

        private abstract class InitExpressionHelperBase<TExpression, TNewExpression> : IInitExpressionHelper
            where TExpression : Expression
            where TNewExpression : Expression
        {
            private readonly Func<TExpression, TNewExpression> _newExpressionFactory;
            private readonly Func<TNewExpression, bool> _parameterlessConstructorTester;

            protected InitExpressionHelperBase(
               Func<TExpression, TNewExpression> newExpressionFactory,
               Func<TNewExpression, bool> parameterlessConstructorTester)
            {
                _newExpressionFactory = newExpressionFactory;
                _parameterlessConstructorTester = parameterlessConstructorTester;
            }

            public string Translate(Expression expression, TranslationContext context)
            {
                var typedExpression = (TExpression)expression;
                var newExpression = GetNewExpressionString(typedExpression, context);
                var memberInitialisations = GetMemberInitialisations(typedExpression, context).ToArray();

                return GetInitialisation(newExpression, memberInitialisations);
            }

            protected virtual string GetNewExpressionString(TExpression initialisation, TranslationContext context)
            {
                var newExpression = _newExpressionFactory.Invoke(initialisation);
                var newExpressionString = context.Translate(newExpression);

                if (ConstructorIsParameterless(newExpression))
                {
                    // Remove the empty brackets:
                    newExpressionString = newExpressionString.Substring(0, newExpressionString.Length - 2);
                }

                return newExpressionString;
            }

            private bool ConstructorIsParameterless(TNewExpression newExpression)
            {
                return _parameterlessConstructorTester.Invoke(newExpression);
            }

            protected abstract IEnumerable<string> GetMemberInitialisations(TExpression initialisation, TranslationContext context);

            protected static string GetInitialisation(string newExpression, string[] memberInitialisations)
            {
                if (memberInitialisations.Length == 0)
                {
                    if (newExpression.EndsWith(')') || newExpression.EndsWith(']'))
                    {
                        return newExpression;
                    }

                    return newExpression + "()";
                }

                if ((newExpression.Length + memberInitialisations.Sum(init => init.Length + 2)) <= 40)
                {
                    return $"{newExpression} {{ {string.Join(", ", memberInitialisations)} }}";
                }

                var initialisationBlock = string.Join(
                    "," + Environment.NewLine,
                    memberInitialisations.Select(init => init.Indented()));

                var initialisation = $@"
{newExpression}
{{
{initialisationBlock}
}}";
                return initialisation.TrimStart();
            }
        }
    }
}
