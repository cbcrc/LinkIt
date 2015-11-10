using System;
using HeterogeneousDataSources.Protocols;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IIncludeWithCreateNestedLinkedSourceById<TLinkedSource, TIChildLinkedSource, TLink>:IInclude
    {
        Type ReferenceType { get; }

        TIChildLinkedSource CreateNestedLinkedSourceById(
            TLink link, 
            LoadedReferenceContext loadedReferenceContext, 
            TLinkedSource linkedSource, 
            int referenceIndex
        );
        
    }
}