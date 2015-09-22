using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface INestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink>: IInclude
    {
        void AddLookupIds(TLink link, LookupIdContext lookupIdContext);
        List<TIChildLinkedSource> CreateChildLinkedSources(TLink link, LoadedReferenceContext loadedReferenceContext, TLinkedSource linkedSource, int referenceIndex);
        
    }
}