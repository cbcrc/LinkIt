using System;

namespace LinkIt.Core.Interfaces
{
    public interface ILinkedSourceConfig {
        Type LinkedSourceType { get; }
        Type LinkedSourceModelType { get; }
    }
}