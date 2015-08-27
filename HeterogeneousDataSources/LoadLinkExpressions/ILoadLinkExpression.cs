using System;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public interface ILoadLinkExpression
    {
        Type LinkedSourceType { get; }
        Type ReferenceType { get; }
        bool IsNestedLinkedSourceLoadLinkExpression { get; }
        void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext);
        void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext);
    }
}