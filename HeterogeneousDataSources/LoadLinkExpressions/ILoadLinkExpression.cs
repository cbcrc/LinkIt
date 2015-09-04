using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public interface ILoadLinkExpression
    {
        Type LinkedSourceType { get; }
        List<Type> ReferenceTypes { get; }
        LoadLinkExpressionType LoadLinkExpressionType { get; }

        void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext, Type referenceTypeToBeLoaded);
        void Link(object linkedSource, LoadedReferenceContext loadedReferenceContext, Type referenceTypeToBeLinked);
        
    }
}