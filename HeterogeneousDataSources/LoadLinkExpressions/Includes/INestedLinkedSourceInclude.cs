using System;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface INestedLinkedSourceInclude<TLinkedSource, TIChildLinkedSource, TLink> : IInclude, IWithAddLookupId<TLink>
    {
        TIChildLinkedSource CreateChildLinkedSource(
            TLink link, 
            LoadedReferenceContext loadedReferenceContext, 
            TLinkedSource linkedSource, 
            int referenceIndex
        );
        
    }
}