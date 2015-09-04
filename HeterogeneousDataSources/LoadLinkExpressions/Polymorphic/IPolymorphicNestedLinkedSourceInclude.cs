using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions.Polymorphic
{
    public interface IPolymorphicNestedLinkedSourceInclude<TIChildLinkedSource, TLink>
    {
        Type ReferenceType { get; }
        void AddLookupIds(TLink link, LookupIdContext lookupIdContext);
        List<TIChildLinkedSource> CreateChildLinkedSources(TLink link, LoadedReferenceContext loadedReferenceContext);
    }
}