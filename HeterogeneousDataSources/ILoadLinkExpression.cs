using System;

namespace HeterogeneousDataSources
{
    public interface ILoadLinkExpression
    {
        Type ReferenceType { get; }
        void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext);
        void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext);
    }
}