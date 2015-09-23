using System;
using System.Collections.Generic;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public interface ILoadLinkExpression
    {
        string LinkTargetId { get; }

        Type LinkedSourceType { get; }
        List<Type> ReferenceTypes { get; }
        LoadLinkExpressionType LoadLinkExpressionType { get; }

        void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext, Type referenceTypeToBeLoaded);

        void LinkNestedLinkedSource(object linkedSource, LoadedReferenceContext loadedReferenceContext, Type referenceTypeToBeLinked);

        //stle: Is loadedReferenceContext the right place to register sub linked source and nested linked source?
        void LinkSubLinkedSource(object linkedSource, LoadedReferenceContext loadedReferenceContext);
        void LinkReference(object linkedSource, LoadedReferenceContext loadedReferenceContext);
    }
}