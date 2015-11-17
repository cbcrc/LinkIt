using System;
using System.Collections.Generic;
using LinkIt.ReferenceTrees;

namespace LinkIt.Core.Interfaces
{
    public interface ILoadLinkExpression
    {
        string LinkTargetId { get; }

        Type LinkedSourceType { get; }
        List<Type> ReferenceTypes { get; }

        void AddLookupIds(object linkedSource, LookupIdContext lookupIdContext, Type referenceTypeToBeLoaded);
        void LinkNestedLinkedSourceById(object linkedSource, LoadedReferenceContext loadedReferenceContext, Type referenceTypeToBeLinked, LoadLinkProtocol loadLinkProtocol);
        void LinkNestedLinkedSourceFromModel(object linkedSource, LoadedReferenceContext loadedReferenceContext, LoadLinkProtocol loadLinkProtocol);
        void LinkReference(object linkedSource, LoadedReferenceContext loadedReferenceContext);

        void AddReferenceTreeForEachInclude(ReferenceTree parent, LoadLinkProtocol loadLinkProtocol);
    }
}