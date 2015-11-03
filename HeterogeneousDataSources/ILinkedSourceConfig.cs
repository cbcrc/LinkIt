using System;

namespace HeterogeneousDataSources
{
    public interface ILinkedSourceConfig {
        Type LinkedSourceType { get; }
        Type LinkedSourceModelType { get; }
    }
}