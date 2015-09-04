using System;

namespace HeterogeneousDataSources.LoadLinkExpressions.Polymorphic
{
    public class PolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink, TChildLinkedSource, TChildLinkedSourceModel, TId> 
        : IPolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink>
        where TChildLinkedSource:TIChildLinkedSource
    {
        public PolymorphicNestedLinkedSourceInclude(
            Func<TLink, TId> getLookupIdFunc,
            Action<TLink, TChildLinkedSource> initChildLinkedSourceAction=null)
        {
        }
    }
}