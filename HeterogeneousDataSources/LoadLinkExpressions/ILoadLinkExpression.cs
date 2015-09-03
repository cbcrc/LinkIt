using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public interface ILoadLinkExpression
    {
        Type LinkedSourceType { get; }
        List<Type> ReferenceTypes { get; }
        LoadLinkExpressionType LoadLinkExpressionType { get; }
        bool DoesMatchReferenceTypeToBeLoaded(object linkedSource, List<Type> referenceTypesToBeLoaded);
        void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext);
        void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext);
        
    }
}