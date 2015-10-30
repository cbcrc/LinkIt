using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace HeterogeneousDataSources
{
    //Simplification of the solution shown at:
    //http://stackoverflow.com/questions/7723744/expressionfunctmodel-string-to-expressionactiontmodel-getter-to-sette
    public static class LinkTargetFactory {

        #region Single Value
        public static LinkTargetBase<TLinkedSource, TTargetProperty> Create<TLinkedSource, TTargetProperty>(
            Expression<Func<TLinkedSource, TTargetProperty>> linkTargetGetterFunc) {
            PropertyInfo property = GetPropertyFromGetter(linkTargetGetterFunc);

            return new SingleValueLinkTarget<TLinkedSource, TTargetProperty>(
                property.Name,
                CreateSingleValueLinkTargetSetterAction<TLinkedSource, TTargetProperty>(property)
            );
        }

        private static Action<TLinkedSource, TTargetProperty> CreateSingleValueLinkTargetSetterAction<TLinkedSource, TTargetProperty>(PropertyInfo property) {
            MethodInfo setter = property.GetSetMethod();
            return (Action<TLinkedSource, TTargetProperty>)Delegate.CreateDelegate(
                typeof(Action<TLinkedSource, TTargetProperty>),
                setter
            );
        } 
        #endregion

        #region Multi Value
        public static LinkTargetBase<TLinkedSource, TTargetProperty> Create<TLinkedSource, TTargetProperty>(
            Expression<Func<TLinkedSource, List<TTargetProperty>>> linkTargetGetterFunc) {
            PropertyInfo property = GetPropertyFromGetter(linkTargetGetterFunc);

            return new MultiValueLinkTarget<TLinkedSource, TTargetProperty>(
                property.Name,
                linkTargetGetterFunc.Compile(),
                CreateMultiValueLinkTargetSetterAction<TLinkedSource, TTargetProperty>(property)
            );
        }

        private static Action<TLinkedSource, List<TTargetProperty>> CreateMultiValueLinkTargetSetterAction<TLinkedSource, TTargetProperty>(PropertyInfo property) {
            MethodInfo setter = property.GetSetMethod();
            return (Action<TLinkedSource, List<TTargetProperty>>)Delegate.CreateDelegate(
                typeof(Action<TLinkedSource, List<TTargetProperty>>),
                setter
            );
        } 
        #endregion

        #region Shared
        private static PropertyInfo GetPropertyFromGetter<TLinkedSource, TTargetProperty>(Expression<Func<TLinkedSource, TTargetProperty>> getter) {

            EnsureNoFunctionOrAction(getter);

            var getterBody = (MemberExpression)getter.Body;
            EnsureNoNestedProperty(getterBody);
            EnsureNoMemberAccess(getterBody);

            var property = (PropertyInfo)getterBody.Member;

            EnsureNoWriteOnlyProperty(property);
            EnsureNoReadOnlyProperty(property);
            return property;
        }

        private static void EnsureNoReadOnlyProperty(PropertyInfo property) {
            if (property.CanRead == false) {
                throw new ArgumentException("Only read-write property are supported");
            }
        }

        private static void EnsureNoWriteOnlyProperty(PropertyInfo property) {
            if (property.CanWrite == false) {
                throw new ArgumentException("Only read-write property are supported");
            }
        }

        private static void EnsureNoMemberAccess(MemberExpression getterBody) {
            if (getterBody.Member.MemberType != MemberTypes.Property) {
                throw OnlyDirectGetterAreSupported();
            }
        }

        private static void EnsureNoNestedProperty(MemberExpression getterBody) {
            var getterBodyExpression = getterBody.Expression;
            if (getterBodyExpression.NodeType != ExpressionType.Parameter) {
                throw OnlyDirectGetterAreSupported();
            }
        }

        private static void EnsureNoFunctionOrAction<TLinkedSource, TTargetProperty>(Expression<Func<TLinkedSource, TTargetProperty>> getter) {
            if (getter.Body.NodeType != ExpressionType.MemberAccess) {
                throw OnlyDirectGetterAreSupported();
            }
        }

        private static ArgumentException OnlyDirectGetterAreSupported() {
            return new ArgumentException("Only direct getter are supported. Ex: p => p.Property");
        } 
        #endregion
    }
}