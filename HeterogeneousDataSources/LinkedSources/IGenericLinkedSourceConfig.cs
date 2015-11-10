using System;
using System.Collections.Generic;
using HeterogeneousDataSources.LinkTargets;
using HeterogeneousDataSources.LoadLinkExpressions.Includes;
using HeterogeneousDataSources.Protocols;

namespace HeterogeneousDataSources.LinkedSources
{
    public interface IGenericLinkedSourceConfig<TLinkedSource> : ILinkedSourceConfig
    {
        ILoadLinker<TLinkedSource> CreateLoadLinker(
            IReferenceLoader referenceLoader,
            List<List<Type>> referenceTypeToBeLoadedForEachLoadingLevel,
            LoadLinkConfig config
        );

        IInclude CreateNestedLinkedSourceInclude<TLinkTargetOwner, TIChildLinkedSource, TLink, TId>(
            Func<TLink, TId> getLookupIdFunc,
            Action<TLinkTargetOwner, int, TLinkedSource> initChildLinkedSourceAction = null
        );

        IInclude CreateSubLinkedSourceInclude<TIChildLinkedSource, TLink, TChildLinkedSourceModel>(Func<TLink, TChildLinkedSourceModel> getSubLinkedSourceModel, ILinkTarget linkTarget);
    }
}