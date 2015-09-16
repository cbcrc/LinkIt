using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions.Polymorphic;

namespace HeterogeneousDataSources
{
    public class IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>
    {
        private Dictionary<TDiscriminant, IPolymorphicNestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink>> _includeByDiscriminantValue
            = new Dictionary<TDiscriminant, IPolymorphicNestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink>>();

        public IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> When<TChildLinkedSource, TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupIdFunc,
            Action<TLink, TChildLinkedSource> initChildLinkedSourceAction = null,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction2 = null
            ) 
        {
            _includeByDiscriminantValue.Add(
                discriminantValue,
                CreatePolymorphicNestedLinkedSourceInclude(getLookupIdFunc, initChildLinkedSourceAction, initChildLinkedSourceAction2)
            );

            return this;
        }

        private IPolymorphicNestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink> CreatePolymorphicNestedLinkedSourceInclude<TChildLinkedSource, TId>(
            Func<TLink, TId> getLookupIdFunc, 
            Action<TLink, TChildLinkedSource> initChildLinkedSourceAction, 
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction2) 
        {
            Type ctorGenericType = typeof(PolymorphicNestedLinkedSourceInclude<,,,,,>);

            var childLinkedSourceType = typeof (TChildLinkedSource);
            Type[] typeArgs ={
                typeof(TLinkedSource),
                typeof(TIChildLinkedSource), 
                typeof(TLink),
                childLinkedSourceType, 
                LoadLinkProtocolForLinkedSourceBuilder<string>.GetLinkedSourceModelType(childLinkedSourceType),
                typeof(TId)
            };

            Type ctorSpecificType = ctorGenericType.MakeGenericType(typeArgs);

            //stle: change to single once obsolete constructor is deleted
            var ctor = ctorSpecificType.GetConstructors().First();

            return (IPolymorphicNestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink>)ctor.Invoke(
                new object[]{
                    getLookupIdFunc,
                    initChildLinkedSourceAction,
                    initChildLinkedSourceAction2
                }
            );
        }

        public Dictionary<TDiscriminant, IPolymorphicNestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink>> Build() {
            return _includeByDiscriminantValue;
        }
    }
}