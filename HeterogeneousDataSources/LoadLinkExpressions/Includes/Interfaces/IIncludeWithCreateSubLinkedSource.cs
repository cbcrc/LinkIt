using System.Collections.Generic;
using HeterogeneousDataSources.Protocols;
using HeterogeneousDataSources.ReferenceTrees;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes
{
    public interface IIncludeWithCreateSubLinkedSource<TIChildLinkedSource,TLink>:IInclude
    {
        TIChildLinkedSource CreateSubLinkedSource(TLink link, LoadedReferenceContext loadedReferenceContext);

        //stle: interface segregation
        void AddReferenceTreeForEachLinkTarget(ReferenceTree parent, LoadLinkConfig config);
    }
}