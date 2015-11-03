using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IIncludeWithCreateSubLinkedSource<TIChildLinkedSource,TLink>:IInclude
    {
        TIChildLinkedSource CreateSubLinkedSource(TLink link, LoadedReferenceContext loadedReferenceContext);

        //stle: interface segregation
        List<ReferenceTree> CreateReferenceTreeForEachLinkTarget(ReferenceTree parent, LoadLinkConfig config);
    }
}