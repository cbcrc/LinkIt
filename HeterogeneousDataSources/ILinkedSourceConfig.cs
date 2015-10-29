using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public interface ILinkedSourceConfig<TLinkedSource>
    {
        Type LinkedSourceType { get; }

        ILoadLinker<TLinkedSource> CreateLoadLinker(
            IReferenceLoader referenceLoader,
            List<List<Type>> referenceTypeToBeLoadedForEachLoadingLevel,
            LoadLinkConfig config
        );

    }
}