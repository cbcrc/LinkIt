using System;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IWithCreateNestedLinkedSource<TLinkedSource, TIChildLinkedSource, TLink> 
    {
        TIChildLinkedSource CreateNestedLinkedSource(
            TLink link, 
            LoadedReferenceContext loadedReferenceContext, 
            TLinkedSource linkedSource, 
            int referenceIndex
        );
        
    }
}