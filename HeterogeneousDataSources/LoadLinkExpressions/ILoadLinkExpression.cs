using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public interface ILoadLinkExpression
    {
        Type ReferenceType { get; }
        bool IsNestedLinkedSourceLoadLinkExpression { get; }
        void AddLookupIds(List<object> linkedSources, LookupIdContext lookupIdContext);
        void Link(LoadedReferenceContext loadedReferenceContext);
    }
}