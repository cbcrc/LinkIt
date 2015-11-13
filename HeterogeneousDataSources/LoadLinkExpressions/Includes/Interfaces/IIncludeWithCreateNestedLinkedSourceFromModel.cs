using LinkIt.Protocols;
using LinkIt.ReferenceTrees;

namespace LinkIt.LoadLinkExpressions.Includes.Interfaces
{
    public interface IIncludeWithCreateNestedLinkedSourceFromModel<TAbstractChildLinkedSource,TLink>:IInclude
    {
        TAbstractChildLinkedSource CreateNestedLinkedSourceFromModel(TLink link, LoadedReferenceContext loadedReferenceContext);

        //stle: interface segregation
        void AddReferenceTreeForEachLinkTarget(ReferenceTree parent, LoadLinkConfig config);
    }
}