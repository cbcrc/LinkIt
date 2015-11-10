using System;

namespace HeterogeneousDataSources.LinkedSources
{
    public interface ILinkedSourceConfig {
        Type LinkedSourceType { get; }
        Type LinkedSourceModelType { get; }
    }
}