using System;

namespace LinkIt.Core.Includes.Interfaces
{
    public interface IIncludeWithCreateNestedLinkedSourceById<TLinkedSource, TAbstractChildLinkedSource, TLink>:IInclude
    {
        Type ReferenceType { get; }

        TAbstractChildLinkedSource CreateNestedLinkedSourceById(
            TLink link, 
            LoadedReferenceContext loadedReferenceContext, 
            TLinkedSource linkedSource, 
            int referenceIndex,
            LoadLinkProtocol loadLinkProtocol
        );
        
    }
}