using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions.Polymorphic;

namespace HeterogeneousDataSources
{
    public class IncludeBuilder<TIChildLinkedSource, TLink, TDiscriminant>
    {
        private Dictionary<TDiscriminant, IPolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink>> _includeByDiscriminantValue
            = new Dictionary<TDiscriminant, IPolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink>>();

        public IncludeBuilder<TIChildLinkedSource, TLink, TDiscriminant> When<TChildLinkedSource, TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupIdFunc,
            Action<TLink, TChildLinkedSource> initChildLinkedSourceAction = null) 
        {
            _includeByDiscriminantValue.Add(
                discriminantValue,
                CreatePolymorphicNestedLinkedSourceInclude(getLookupIdFunc, initChildLinkedSourceAction)
            );

            return this;
        }

        private IPolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink> CreatePolymorphicNestedLinkedSourceInclude<TChildLinkedSource, TId>(
            Func<TLink, TId> getLookupIdFunc,
            Action<TLink, TChildLinkedSource> initChildLinkedSourceAction = null) 
        {
            Type ctorGenericType = typeof(PolymorphicNestedLinkedSourceInclude<,,,,>);

            var childLinkedSourceType = typeof (TChildLinkedSource);
            Type[] typeArgs ={
                typeof(TIChildLinkedSource), 
                typeof(TLink),
                childLinkedSourceType, 
                LoadLinkProtocolForLinkedSourceBuilder<string>.GetLinkedSourceModelType(childLinkedSourceType),
                typeof(TId)
            };

            Type ctorSpecificType = ctorGenericType.MakeGenericType(typeArgs);

            //stle: change to single once obsolete constructor is deleted
            var ctor = ctorSpecificType.GetConstructors().First();

            return (IPolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink>)ctor.Invoke(
                new object[]{
                    getLookupIdFunc,
                    initChildLinkedSourceAction
                }
            );
        }

        public Dictionary<TDiscriminant, IPolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink>> Build(){
            return _includeByDiscriminantValue;
        }
    }
}