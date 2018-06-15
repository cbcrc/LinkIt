// Copyright (c) 2016 AgileObjects Ltd
// Licensed under the MIT license. See LICENSE file in the ReadableExpressions directory for more information.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LinkIt.ReadableExpressions.Translators
{
    using System.Linq;
    using System.Linq.Expressions;
    using Extensions;

    internal class NewExpressionTranslator : ExpressionTranslatorBase
    {
        internal NewExpressionTranslator()
            : base(ExpressionType.New)
        {
        }

        public override string Translate(Expression expression, TranslationContext context)
        {
            var newExpression = (NewExpression)expression;

            if (IsAnonymous(newExpression.Type))
            {
                return GetAnonymousTypeCreation(newExpression, context);
            }

            var typeName = (newExpression.Type == typeof(object)) ? "Object" : newExpression.Type.GetFriendlyName();
            var parameters = context.TranslateParameters(newExpression.Arguments).WithParentheses();

            return "new " + typeName + parameters;
        }

        private static bool IsAnonymous(Type type)
        {
            return type.IsGenericType &&
                   type.Name.Contains("AnonymousType") &&
                   (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$")) &&
                   (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic &&
                   type.GetCustomAttribute<CompilerGeneratedAttribute>(inherit: false) != null;
        }

        private static string GetAnonymousTypeCreation(NewExpression newExpression, TranslationContext context)
        {
            var constructorParameters = newExpression.Constructor.GetParameters();

            var arguments = newExpression
                .Arguments
                .Select((arg, i) => constructorParameters[i].Name + " = " + context.Translate(arg));

            var argumentsString = string.Join(", ", arguments);

            return "new { " + argumentsString + " }";
        }
    }
}