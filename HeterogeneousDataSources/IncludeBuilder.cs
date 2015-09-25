using System;
using System.Collections.Generic;
using System.Linq;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;

namespace HeterogeneousDataSources
{
    public class IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant>
    {
        private Dictionary<TDiscriminant, IInclude> _includeByDiscriminantValue
            = new Dictionary<TDiscriminant, IInclude>();

        public IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> WhenNestedLinkedSource<TChildLinkedSource, TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupIdFunc,
            Func<TLinkedSource, int, TChildLinkedSource, TChildLinkedSource> initChildLinkedSource = null
            ) 
        {
            _includeByDiscriminantValue.Add(
                discriminantValue,
                CreatePolymorphicNestedLinkedSourceInclude(getLookupIdFunc, initChildLinkedSource)
            );

            return this;
        }

        public IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> WhenSubLinkedSource<TChildLinkedSource>(
            TDiscriminant discriminantValue) 
        {
            _includeByDiscriminantValue.Add(
                discriminantValue,
                CreatePolymorphicSubLinkedSourceInclude<TChildLinkedSource>()
            );

            return this;
        }

        private IInclude CreatePolymorphicSubLinkedSourceInclude<TChildLinkedSource>(){
            Type ctorGenericType = typeof(SubLinkedSourceInclude<,,>);

            var iChildLinkedSourceType = typeof (TIChildLinkedSource); //stle:dry
            var childLinkedSourceType = typeof(TChildLinkedSource);

            Type[] typeArgs ={
                iChildLinkedSourceType,
                childLinkedSourceType,
                //stle: move this out
                LoadLinkProtocolForLinkedSourceBuilder<string>.GetLinkedSourceModelType(childLinkedSourceType)
            };

            Type ctorSpecificType = ctorGenericType.MakeGenericType(typeArgs);

            //stle: change to single once obsolete constructor is deleted
            var ctor = ctorSpecificType.GetConstructors().First();

            return (IInclude)ctor.Invoke(new object[]{});
        }


        private IInclude CreatePolymorphicNestedLinkedSourceInclude<TChildLinkedSource, TId>(
            Func<TLink, TId> getLookupIdFunc,
            Func<TLinkedSource, int, TChildLinkedSource, TChildLinkedSource> initChildLinkedSource) 
        {
            Type ctorGenericType = typeof(NestedLinkedSourceInclude<,,,,,>);

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

            return (IInclude)ctor.Invoke(
                new object[]{
                    getLookupIdFunc,
                    initChildLinkedSource
                }
            );
        }

        public Dictionary<TDiscriminant, IInclude> Build() {
            return _includeByDiscriminantValue;
        }
    }
}