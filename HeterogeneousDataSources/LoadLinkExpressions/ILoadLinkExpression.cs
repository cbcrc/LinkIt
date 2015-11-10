using System;
using System.Collections.Generic;
using HeterogeneousDataSources.Protocols;
using HeterogeneousDataSources.ReferenceTrees;

namespace HeterogeneousDataSources.LoadLinkExpressions
{
    //stle: may go away or be simplified to a strict minimum
    public interface ILoadLinkExpression
    {
        string LinkTargetId { get; }

        Type LinkedSourceType { get; }
        List<Type> ReferenceTypes { get; }

        void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext, Type referenceTypeToBeLoaded);

        void LinkNestedLinkedSourceById(object linkedSource, LoadedReferenceContext loadedReferenceContext, Type referenceTypeToBeLinked);

        //stle: Is loadedReferenceContext the right place to register sub linked source and nested linked source?
        void LinkNestedLinkedSourceFromModel(object linkedSource, LoadedReferenceContext loadedReferenceContext);
        void LinkReference(object linkedSource, LoadedReferenceContext loadedReferenceContext);

        void AddReferenceTreeForEachInclude(ReferenceTree parent, LoadLinkConfig config);
    }
}