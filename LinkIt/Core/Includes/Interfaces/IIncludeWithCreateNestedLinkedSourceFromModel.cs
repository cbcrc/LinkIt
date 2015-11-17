using LinkIt.ReferenceTrees;

namespace LinkIt.Core.Includes.Interfaces
{
    public interface IIncludeWithCreateNestedLinkedSourceFromModel<TAbstractChildLinkedSource,TLink>:IInclude
    {
        TAbstractChildLinkedSource CreateNestedLinkedSourceFromModel(TLink link, LoadedReferenceContext loadedReferenceContext, LoadLinkProtocol loadLinkProtocol);

        void AddReferenceTreeForEachLinkTarget(ReferenceTree parent, LoadLinkProtocol loadLinkProtocol);
    }
}