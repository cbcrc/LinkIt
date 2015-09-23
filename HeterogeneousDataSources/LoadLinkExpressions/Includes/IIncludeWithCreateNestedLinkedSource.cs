using System;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IIncludeWithCreateNestedLinkedSource<TLinkedSource, TIChildLinkedSource, TLink>:IInclude
    {
        TIChildLinkedSource CreateNestedLinkedSource(
            TLink link, 
            LoadedReferenceContext loadedReferenceContext, 
            TLinkedSource linkedSource, 
            int referenceIndex
        );
        
    }
}