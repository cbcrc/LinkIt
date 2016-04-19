#region copyright
// Copyright (c) CBC/Radio-Canada. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
#endregion

using System;
using System.Linq.Expressions;

namespace LinkIt.Conventions {
    public static class FuncGenerator {
        public static Func<TPropertyOwner, TProperty> GenerateFromGetter<TPropertyOwner, TProperty>(string propertyInDotNotation) {
            return GenerateFromGetterAsExpression<TPropertyOwner, TProperty>(propertyInDotNotation).Compile();
        }
        
        public static Expression<Func<TPropertyOwner, TProperty>> GenerateFromGetterAsExpression<TPropertyOwner, TProperty>(string propertyInDotNotation) {
            var root = Expression.Parameter(typeof(TPropertyOwner), "root");
            var lambdaBody = GenerateGetProperty(root, propertyInDotNotation);
            return Expression.Lambda<Func<TPropertyOwner, TProperty>>(lambdaBody, root);
        }

        private static Expression GenerateGetProperty(ParameterExpression root, string propertyInDotNotation) {
            Expression propertyExpression = root;
            foreach (var property in propertyInDotNotation.Split('.')) {
                propertyExpression = Expression.PropertyOrField(propertyExpression, property);
            }
            return propertyExpression;
        }
    }
}
