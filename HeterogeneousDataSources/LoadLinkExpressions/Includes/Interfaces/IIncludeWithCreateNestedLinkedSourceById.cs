using System;
using HeterogeneousDataSources.Protocols;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IIncludeWithCreateNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink>:IInclude
    {
        Type ReferenceType { get; }

        TAbstractChildLinkedSource CreateNestedLinkedSourceById(
            TLink link, 
            LoadedReferenceContext loadedReferenceContext, 
            TLinkedSource linkedSource, 
            int referenceIndex
        );
        
    }
}