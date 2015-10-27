using System;

namespace HeterogeneousDataSources
{
    public interface ILinkedSourceConfig<TLinkedSource>
    {
        Type LinkedSourceType { get; }
        //stle: required?
        Type LinkedSourceModelType { get; }
        ILoadLinker<TLinkedSource> CreateLoadLinker(IReferenceLoader referenceLoader);
    }
}