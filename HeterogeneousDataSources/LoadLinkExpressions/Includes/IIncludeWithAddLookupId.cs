using System;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes {

    public interface IIncludeWithAddLookupId<TLink>:IInclude {
        Type ReferenceType { get; }
        void AddLookupId(TLink link, LookupIdContext lookupIdContext);
        ReferenceTree CreateReferenceTree(string linkTargetId, ReferenceTree parent, LoadLinkConfig config);
    }
}
