using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions.Polymorphic;

namespace HeterogeneousDataSources
{
    public class IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>
    {
        private Dictionary<TDiscriminant, IPolymorphicInclude> _includeByDiscriminantValue
            = new Dictionary<TDiscriminant, IPolymorphicInclude>();

        public IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> When<TChildLinkedSource, TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupIdFunc,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction = null
            ) 
        {
            _includeByDiscriminantValue.Add(
                discriminantValue,
                CreatePolymorphicNestedLinkedSourceInclude(getLookupIdFunc, initChildLinkedSourceAction)
            );

            return this;
        }

        public IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> WhenSub<TChildLinkedSource>(
            TDiscriminant discriminantValue) 
        {
            //_includeByDiscriminantValue.Add(
            //    discriminantValue,
            //    null
            //);

            return this;
        }


        private IPolymorphicInclude CreatePolymorphicNestedLinkedSourceInclude<TChildLinkedSource, TId>(
            Func<TLink, TId> getLookupIdFunc, 
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction) 
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

            return (IPolymorphicInclude)ctor.Invoke(
                new object[]{
                    getLookupIdFunc,
                    initChildLinkedSourceAction
                }
            );
        }

        public Dictionary<TDiscriminant, IPolymorphicInclude> Build() {
            return _includeByDiscriminantValue;
        }
    }
}