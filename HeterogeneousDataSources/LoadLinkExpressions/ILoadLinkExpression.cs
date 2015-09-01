using System;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public interface ILoadLinkExpression
    {
        Type LinkedSourceType { get; }
        Type ReferenceType { get; }
        Type ModelType { get; }
        LoadLinkExpressionType LoadLinkExpressionType { get; }
        void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext);
        void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext);
    }
}