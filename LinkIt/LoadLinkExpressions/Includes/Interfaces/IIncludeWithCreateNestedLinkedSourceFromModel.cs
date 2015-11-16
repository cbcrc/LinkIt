using LinkIt.Protocols;
using LinkIt.ReferenceTrees;

namespace LinkIt.LoadLinkExpressions.Includes.Interfaces
{
    public interface IIncludeWithCreateNestedLinkedSourceFromModel<TAbstractChildLinkedSource,TLink>:IInclude
    {
        TAbstractChildLinkedSource CreateNestedLinkedSourceFromModel(TLink link, LoadedReferenceContext loadedReferenceContext, LoadLinkProtocol loadLinkProtocol);

        //stle: interface segregation
        void AddReferenceTreeForEachLinkTarget(ReferenceTree parent, LoadLinkProtocol loadLinkProtocol);
    }
}