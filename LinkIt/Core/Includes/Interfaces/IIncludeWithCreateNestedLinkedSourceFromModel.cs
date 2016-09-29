using LinkIt.ReferenceTrees;

namespace LinkIt.Core.Includes.Interfaces
{
    public interface IIncludeWithCreateNestedLinkedSourceFromModel<TLinkedSource,TAbstractChildLinkedSource, TLink>:IInclude
    {
        TAbstractChildLinkedSource CreateNestedLinkedSourceFromModel(
            TLink link, 
            LoadedReferenceContext loadedReferenceContext, 
            TLinkedSource linkedSource,
            int referenceIndex,
            LoadLinkProtocol loadLinkProtocol
        );

        void AddReferenceTreeForEachLinkTarget(ReferenceTree parent, LoadLinkProtocol loadLinkProtocol);
    }
}