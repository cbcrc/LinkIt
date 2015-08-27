using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources
{
    public interface ILoadLinkExpression
    {
        Type ReferenceType { get; }
        bool IsNestedLinkedSourceLoadLinkExpression { get; }
        void AddLookupIds(List<object> linkedSources, LookupIdContext lookupIdContext);
        void Link(List<object> linkedSources, LoadedReferenceContext loadedReferenceContext);
    }
}