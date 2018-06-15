﻿// Copyright (c) 2016 AgileObjects Ltd
// Licensed under the MIT license. See LICENSE file in the ReadableExpressions directory for more information.

namespace LinkIt.ReadableExpressions.Translators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Extensions;

    internal partial class InitialisationExpressionTranslator
    {
        private class ArrayInitExpressionHelper : InitExpressionHelperBase<NewArrayExpression, NewArrayExpression>
        {
            public ArrayInitExpressionHelper()
                : base(null, null)
            {
            }

            protected override string GetNewExpressionString(NewArrayExpression initialisation, TranslationContext context)
            {
                if (initialisation.Expressions.Count == 0)
                {
                    return "new " + GetExplicitArrayType(initialisation) + "[0]";
                }

                var explicitType = GetExplicitArrayTypeIfRequired(initialisation);

                return "new" + explicitType + "[]";
            }

            private static string GetExplicitArrayType(Expression initialisation)
            {
                return initialisation.Type.GetElementType().GetFriendlyName();
            }

            private static string GetExplicitArrayTypeIfRequired(NewArrayExpression initialisation)
            {
                var expressionTypes = initialisation
                    .Expressions
                    .Select(exp => exp.Type)
                    .Distinct()
                    .ToArray();

                if (expressionTypes.Length == 1)
                {
                    return null;
                }

                return " " + GetExplicitArrayType(initialisation);
            }

            protected override IEnumerable<string> GetMemberInitialisations(
                NewArrayExpression arrayInitialisation,
                TranslationContext context)
            {
                return arrayInitialisation.Expressions.Select(context.TranslateAsCodeBlock);
            }
        }
    }
}
