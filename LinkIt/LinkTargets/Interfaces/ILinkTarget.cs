using System;

namespace LinkIt.LinkTargets.Interfaces
{
    public interface ILinkTarget : IEquatable<ILinkTarget> {
        string Id { get; }
    }

    public interface ILinkTarget<TLinkedSource, TTargetProperty> : ILinkTarget {
        //The parameter linkTargetValueIndex is essential to support the 
        //following use case. When list with polymorhic items is linked, 
        //if some items are nested linked source, it's possible those items
        //are not part of the same loading level as the other items. This would
        //occurs if the other items have a dependency on the nested linked source.
        //In this scenario, we need to link item at a specific linkTargetValueIndex.
        //
        //Example: PolymorphicList_WithDependenciesBetweenItemsTests
        void SetLinkTargetValue(
            TLinkedSource linkedSource,
            TTargetProperty linkTargetValue,
            int linkTargetValueIndex
        );

        void LazyInit(TLinkedSource linkedSource, int numOfLinkedTargetValues);

        void FilterOutNullValues(TLinkedSource linkedSource);
    }
}