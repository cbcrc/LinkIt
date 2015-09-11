using System;

namespace HeterogeneousDataSources
{
    public interface ILinkTarget:IEquatable<ILinkTarget>{
        Type LinkedSourceType { get; }
        string PropertyName { get; }
    }
}