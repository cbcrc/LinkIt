using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions.Polymorphic
{
    public interface IPolymorphicNestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink>: IPolymorphicInclude
    {
        void AddLookupIds(TLink link, LookupIdContext lookupIdContext);
        List<TIChildLinkedSource> CreateChildLinkedSources(TLink link, LoadedReferenceContext loadedReferenceContext, TLinkedSource linkedSource, int referenceIndex);
        
    }
}