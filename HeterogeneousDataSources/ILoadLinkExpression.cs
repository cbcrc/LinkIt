using System;

namespace HeterogeneousDataSources
{
    public interface ILoadLinkExpression
    {
        Type ReferenceType { get; }
        void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext);
    }
}