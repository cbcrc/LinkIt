// Copyright (c) 2016 AgileObjects Ltd
// Licensed under the MIT license. See LICENSE file in the ReadableExpressions directory for more information.

namespace LinkIt.ReadableExpressions.Translators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Extensions;

    internal class CastExpressionTranslator : ExpressionTranslatorBase
    {
        private static readonly Dictionary<ExpressionType, Translator> _translatorsByType =
            new Dictionary<ExpressionType, Translator>
            {
                [ExpressionType.Convert] = TranslateCast,
                [ExpressionType.ConvertChecked] = TranslateCast,
                [ExpressionType.TypeAs] = TranslateTypeAs,
                [ExpressionType.TypeIs] = TranslateTypeIs,
                [ExpressionType.Unbox] = TranslateCastCore,
            };

        internal CastExpressionTranslator()
            : base(_translatorsByType.Keys.ToArray())
        {
        }

        public override string Translate(Expression expression, TranslationContext context)
        {
            return _translatorsByType[expression.NodeType].Invoke(expression, context);
        }

        private static string TranslateCast(Expression expression, TranslationContext context)
        {
            var operand = ((UnaryExpression)expression).Operand;

            if (expression.Type == typeof(object))
            {
                // Don't bother showing a boxing operation:
                return context.Translate(operand);
            }

            MethodCallExpression methodCall;

            if ((operand.NodeType == ExpressionType.Call) &&
                (operand.Type == typeof(Delegate)) &&
                ((methodCall = ((MethodCallExpression)operand)).Method.Name == "CreateDelegate"))
            {
                // ReSharper disable once PossibleNullReferenceException
                var subjectMethod = (MethodInfo)((ConstantExpression)methodCall.Object).Value;

                var methodSubject = subjectMethod.IsStatic
                    ? subjectMethod.DeclaringType.GetFriendlyName()
                    : context.Translate(methodCall.Arguments.ElementAtOrDefault(1));

                return methodSubject + "." + subjectMethod.Name;
            }

            return TranslateCastCore(expression, context);
        }

        private static string TranslateCastCore(Expression expression, TranslationContext context)
        {
            var cast = (UnaryExpression)expression;

            return Translate(cast.Operand, cast.Method, cast.Type, context);
        }

        public static string Translate(
            Expression castValue,
            MethodInfo castMethod,
            Type resultType,
            TranslationContext context)
        {
            var typeName = resultType.GetFriendlyName();
            var subject = context.Translate(castValue);

            if (castValue.NodeType == ExpressionType.Assign)
            {
                subject = subject.WithSurroundingParentheses();
            }

            if (castMethod?.IsImplicitOperator() == true)
            {
                return subject;
            }

            return $"(({typeName}){subject})";
        }

        private static string TranslateTypeAs(Expression expression, TranslationContext context)
        {
            var typeAs = (UnaryExpression)expression;
            var typeName = typeAs.Type.GetFriendlyName();
            var subject = context.Translate(typeAs.Operand);

            return $"({subject} as {typeName})";
        }

        private static string TranslateTypeIs(Expression expression, TranslationContext context)
        {
            var typeIs = (TypeBinaryExpression)expression;
            var typeName = typeIs.TypeOperand.GetFriendlyName();
            var subject = context.Translate(typeIs.Expression);

            return $"({subject} is {typeName})";
        }
    }
}