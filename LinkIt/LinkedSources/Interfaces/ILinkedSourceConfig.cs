using System;

namespace LinkIt.LinkedSources.Interfaces
{
    public interface ILinkedSourceConfig {
        Type LinkedSourceType { get; }
        Type LinkedSourceModelType { get; }
    }
}