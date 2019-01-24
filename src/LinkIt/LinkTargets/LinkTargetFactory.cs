// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.ReadableExpressions;
using LinkIt.ReadableExpressions.Extensions;
using LinkIt.Shared;

namespace LinkIt.LinkTargets
{
    //Simplification of the solution shown at:
    //http://stackoverflow.com/questions/7723744/expressionfunctmodel-string-to-expressionactiontmodel-getter-to-sette
    internal static class LinkTargetFactory
    {
        public static ILinkTarget<TLinkedSource, TTargetProperty> Create<TLinkedSource, TTargetProperty>(
            Expression<Func<TLinkedSource, TTargetProperty>> getLinkTarget)
        {
            var property = GetPropertyFromGetter(getLinkTarget);

            return new SingleValueLinkTarget<TLinkedSource, TTargetProperty>(property, getLinkTarget);
        }

        public static ILinkTarget<TLinkedSource, TTargetProperty> Create<TLinkedSource, TTargetProperty>(
            Expression<Func<TLinkedSource, List<TTargetProperty>>> getLinkTarget)
        {
            var property = GetPropertyFromGetter(getLinkTarget);

            return new MultiValueLinkTarget<TLinkedSource, TTargetProperty>(property, getLinkTarget);
        }

        private static PropertyInfo GetPropertyFromGetter<TLinkedSource, TTargetProperty>(Expression<Func<TLinkedSource, TTargetProperty>> getter)
        {
            EnsureIsMember(getter);

            var getterBody = (MemberExpression) getter.Body;
            EnsureIsPropertyOfLinkedSource<TLinkedSource>(getterBody);

            var property = (PropertyInfo) getterBody.Member;
            EnsureIsReadWrite(getter, property);

            //Impossible to have a write only property, since the setter is inferred from the getter

            return property;
        }

        private static void EnsureIsMember<TLinkedSource, TTargetProperty>(Expression<Func<TLinkedSource, TTargetProperty>> getter)
        {
            if (getter.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw OnlyDirectGetterAreSupported<TLinkedSource>(getter);
            }
        }

        private static void EnsureIsPropertyOfLinkedSource<TLinkedSource>(MemberExpression getterBody)
        {
            if (getterBody.Member.MemberType != MemberTypes.Property || (getterBody.Expression.NodeType != ExpressionType.Parameter && getterBody.Expression.NodeType != ExpressionType.Convert))
            {
                throw OnlyDirectGetterAreSupported<TLinkedSource>(getterBody.Expression);
            }
        }

        private static void EnsureIsReadWrite<TLinkedSource, TTargetProperty>(Expression<Func<TLinkedSource, TTargetProperty>> getter, PropertyInfo property)
        {
            if (!property.IsPublicReadWrite())
            {
                var errorMessage = $"Invalid configuration for linked source {typeof(TLinkedSource).GetFriendlyName()}: "
                                   + $"Link {{{getter.ToReadableString()}}} is not valid, only read-write properties are supported.";
                throw new LinkItException(errorMessage);
            }
        }

        private static LinkItException OnlyDirectGetterAreSupported<TLinkedSource>(Expression expression)
        {
            var errorMessage = $"Invalid configuration for linked source {typeof(TLinkedSource).GetFriendlyName()}: "
                             + $"Link {{{expression.ToReadableString()}}} is not valid, only direct getters are supported. Ex: p => p.Property";
            return new LinkItException(errorMessage);
        }
    }
}
