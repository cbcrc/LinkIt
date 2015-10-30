using System;

namespace HeterogeneousDataSources
{
    public interface ILinkTarget:IEquatable<ILinkTarget>{
        string Id { get; }
        Type LinkedSourceType { get; }
        string PropertyName { get; }
    }
}