using System;
using HeterogeneousDataSources.Protocols;
using HeterogeneousDataSources.ReferenceTrees;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes {

    public interface IIncludeWithAddLookupId<TLink>:IInclude {
        Type ReferenceType { get; }
        void AddLookupId(TLink link, LookupIdContext lookupIdContext);
        void AddReferenceTree(string linkTargetId, ReferenceTree parent, LoadLinkConfig config);
    }
}
