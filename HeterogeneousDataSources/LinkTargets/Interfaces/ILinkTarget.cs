using System;

namespace HeterogeneousDataSources.LinkTargets
{
    public interface ILinkTarget : IEquatable<ILinkTarget> {
        string Id { get; }
    }

    public interface ILinkTarget<TLinkedSource, TTargetProperty> : ILinkTarget {
        void SetLinkTargetValue(
            TLinkedSource linkedSource,
            TTargetProperty linkTargetValue,
            int linkTargetValueIndex
        );

        void LazyInit(TLinkedSource linkedSource, int numOfLinkedTargetValues);
    }
}