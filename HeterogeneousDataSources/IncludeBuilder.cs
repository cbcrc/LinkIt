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

        //stle: improve usability
        public IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> WhenNestedLinkedSource<TChildLinkedSource, TId>(
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

        private IInclude CreatePolymorphicNestedLinkedSourceInclude<TChildLinkedSource, TId>(
            Func<TLink, TId> getLookupIdFunc,
            Action<TLinkedSource, int, TChildLinkedSource> initChildLinkedSourceAction) {
            Type ctorGenericType = typeof(NestedLinkedSourceInclude<,,,,,>);

            var childLinkedSourceType = typeof(TChildLinkedSource);
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
                    initChildLinkedSourceAction
                }
            );
        }

        //stle: improve usability
        public IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> WhenSubLinkedSource<TChildLinkedSource, TChildLinkedSourceModel>(
            TDiscriminant discriminantValue,
            Func<TLink, TChildLinkedSourceModel> getSubLinkedSourceModel = null
        )
            where TChildLinkedSource : class, TIChildLinkedSource, ILinkedSource<TChildLinkedSourceModel>, new()
        {
            _includeByDiscriminantValue.Add(
                discriminantValue,
                new SubLinkedSourceInclude<TIChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel>(
                    getSubLinkedSourceModel
                )
            );

            return this;
        }

        //stle: improve usability
        public IncludeBuilder<TLinkedSource, TIChildLinkedSource, TLink, TDiscriminant> WhenReference<TReference, TId>(
            TDiscriminant discriminantValue,
            Func<TLink, TId> getLookupIdFunc
        )
            where TReference:TIChildLinkedSource
        {
            _includeByDiscriminantValue.Add(
                discriminantValue,
                new ReferenceInclude<TIChildLinkedSource, TLink, TReference, TId>(
                    getLookupIdFunc
                )
            );

            return this;
        }

        public Dictionary<TDiscriminant, IInclude> Build() {
            return _includeByDiscriminantValue;
        }
    }
}