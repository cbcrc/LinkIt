// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using LinkIt.LinkTargets.Interfaces;
using LinkIt.Shared;

namespace LinkIt.LinkTargets
{
    //Simplification of the solution shown at:
    //http://stackoverflow.com/questions/7723744/expressionfunctmodel-string-to-expressionactiontmodel-getter-to-sette
    internal static class LinkTargetFactory
    {
        #region Single Value

        public static ILinkTarget<TLinkedSource, TTargetProperty> Create<TLinkedSource, TTargetProperty>(
            Expression<Func<TLinkedSource, TTargetProperty>> getLinkTarget)
        {
            var property = GetPropertyFromGetter(getLinkTarget);

            return new SingleValueLinkTarget<TLinkedSource, TTargetProperty>(property);
        }

        #endregion

        #region Multi Value

        public static ILinkTarget<TLinkedSource, TTargetProperty> Create<TLinkedSource, TTargetProperty>(
            Expression<Func<TLinkedSource, IList<TTargetProperty>>> getLinkTarget)
        {
            var property = GetPropertyFromGetter(getLinkTarget);
            EnsureIsSupportedIListImplementation<TTargetProperty>(property);

            return new MultiValueLinkTarget<TLinkedSource, TTargetProperty>(property, getLinkTarget.Compile());
        }

        private static void EnsureIsSupportedIListImplementation<TTargetProperty>(PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            if (!propertyType.IsAssignableFrom(typeof(TTargetProperty[])) && !propertyType.IsAssignableFrom(typeof(List<TTargetProperty>)))
            {
                throw new ArgumentException($"Property {property.GetFullName()} is an invalid link target for a list: Only properties that can be assigned from an {typeof(TTargetProperty).Name}[] or List<{typeof(TTargetProperty).Name}> are supported.");
            }
        }

        #endregion

        #region Shared

        private static PropertyInfo GetPropertyFromGetter<TLinkedSource, TTargetProperty>(Expression<Func<TLinkedSource, TTargetProperty>> getter)
        {
            EnsureIsMember(getter);

            var getterBody = (MemberExpression) getter.Body;
            EnsureIsPropertyOfLinkedSource<TLinkedSource>(getterBody);

            var property = (PropertyInfo) getterBody.Member;

            EnsureIsReadWrite(property);

            //Impossible to have a write only property, since the setter is inferred from the getter

            return property;
        }

        private static void EnsureIsMember<TLinkedSource, TTargetProperty>(Expression<Func<TLinkedSource, TTargetProperty>> getter)
        {
            if (getter.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw OnlyDirectGetterAreSupported<TLinkedSource>();
            }
        }

        private static void EnsureIsPropertyOfLinkedSource<TLinkedSource>(MemberExpression getterBody)
        {
            if (getterBody.Member.MemberType != MemberTypes.Property || getterBody.Expression.NodeType != ExpressionType.Parameter)
            {
                throw OnlyDirectGetterAreSupported<TLinkedSource>();
            }
        }

        private static void EnsureIsReadWrite(PropertyInfo property)
        {
            if (!property.IsPublicReadWrite())
            {
                throw new ArgumentException($"{property.GetFullName()}: Only read-write properties are supported.");
            }
        }

        private static ArgumentException OnlyDirectGetterAreSupported<TLinkedSource>()
        {
            return new ArgumentException($"{typeof(TLinkedSource)}: Only direct getters are supported. Ex: p => p.Property");
        }

        #endregion
    }
}