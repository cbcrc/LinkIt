// Copyright (c) 2016 AgileObjects Ltd
// Licensed under the MIT license. See LICENSE file in the ReadableExpressions directory for more information.

namespace LinkIt.ReadableExpressions.Translators
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Implementing classes will translate a particular type of Expression.
    /// </summary>
    internal interface IExpressionTranslator
    {
        /// <summary>
        /// Gets the <see cref="ExpressionType"/>s translated by the <see cref="IExpressionTranslator"/>.
        /// </summary>
        IEnumerable<ExpressionType> NodeTypes
        {
            get;
        }

        /// <summary>
        /// Translates the given <paramref name="expression"/> into readable source code.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to translate.</param>
        /// <param name="context">
        /// The <see cref="TranslationContext"/> for the root <see cref="Expression"/> being translated.
        /// </param>
        /// <returns>A source code translation of the given <paramref name="expression"/>.</returns>
        string Translate(Expression expression, TranslationContext context);
    }
}