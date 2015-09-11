using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HeterogeneousDataSources
{
    //Adapted from:
    //http://stackoverflow.com/questions/7723744/expressionfunctmodel-string-to-expressionactiontmodel-getter-to-sette
    public static class LinkTargetFactory {

        public static LinkTarget<TLinkedSource, TTargetProperty> Create<TLinkedSource, TTargetProperty>(
            Expression<Func<TLinkedSource, TTargetProperty>> linkTargetGetterFunc) 
        {
            PropertyInfo property = GetPropertyFromGetter(linkTargetGetterFunc);
            EnsureNoWriteOnlyProperty(property);
            EnsureNoReadOnlyProperty(property);

            return new LinkTarget<TLinkedSource, TTargetProperty>(
                property.Name,
                linkTargetGetterFunc.Compile(),
                CreateLinkTargetSetterAction<TLinkedSource, TTargetProperty>(property)
            );
        }

        private static Action<TLinkedSource, TTargetProperty> CreateLinkTargetSetterAction<TLinkedSource, TTargetProperty>(PropertyInfo property)
        {
            MethodInfo setter = property.GetSetMethod();
            return (Action<TLinkedSource, TTargetProperty>) Delegate.CreateDelegate(
                typeof (Action<TLinkedSource, TTargetProperty>),
                setter
            );
        }

        private static PropertyInfo GetPropertyFromGetter<TLinkedSource, TTargetProperty>(Expression<Func<TLinkedSource, TTargetProperty>> getter) {
            EnsureNoFunctionOrAction(getter);

            var getterBody = (MemberExpression)getter.Body;
            EnsureNoNestedProperty(getterBody);
            EnsureNoMemberAccess(getterBody);

            return (PropertyInfo) getterBody.Member;
        }

        private static void EnsureNoReadOnlyProperty(PropertyInfo property)
        {
            if (property.CanRead == false){
                throw new ArgumentException("Only read-write property are supported");
            }
        }

        private static void EnsureNoWriteOnlyProperty(PropertyInfo property)
        {
            if (property.CanWrite == false){
                throw new ArgumentException("Only read-write property are supported");
            }
        }

        private static void EnsureNoMemberAccess(MemberExpression getterBody)
        {
            if (getterBody.Member.MemberType != MemberTypes.Property){
                throw OnlyDirectGetterAreSupported();
            }
        }

        private static void EnsureNoNestedProperty(MemberExpression getterBody)
        {
            var getterBodyExpression = getterBody.Expression;
            if (getterBodyExpression.NodeType != ExpressionType.Parameter){
                throw OnlyDirectGetterAreSupported();
            }
        }

        private static void EnsureNoFunctionOrAction<TLinkedSource, TTargetProperty>(Expression<Func<TLinkedSource, TTargetProperty>> getter)
        {
            if (getter.Body.NodeType != ExpressionType.MemberAccess){
                throw OnlyDirectGetterAreSupported();
            }
        }

        private static ArgumentException OnlyDirectGetterAreSupported() {
            return new ArgumentException("Only direct getter are supported. Ex: p => p.Property");
        }
    }
}