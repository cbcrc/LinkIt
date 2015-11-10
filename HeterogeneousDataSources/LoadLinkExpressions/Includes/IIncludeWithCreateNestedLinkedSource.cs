using System;
using HeterogeneousDataSources.Protocols;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IIncludeWithCreateNestedLinkedSource<TLinkedSource, TIChildLinkedSource, TLink>:IInclude
    {
        Type ReferenceType { get; }

        TIChildLinkedSource CreateNestedLinkedSource(
            TLink link, 
            LoadedReferenceContext loadedReferenceContext, 
            TLinkedSource linkedSource, 
            int referenceIndex
        );
        
    }
}