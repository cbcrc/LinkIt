using System;
using System.Collections.Generic;
using LinkIt.Protocols;
using LinkIt.ReferenceTrees;

namespace LinkIt.LoadLinkExpressions
{
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