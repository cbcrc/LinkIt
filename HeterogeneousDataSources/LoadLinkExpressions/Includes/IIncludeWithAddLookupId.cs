using System;

namespace HeterogeneousDataSources.LoadLinkExpressions.Includes {

    public interface IIncludeWithAddLookupId<TLink>:IInclude {
        Type ReferenceType { get; }
        void AddLookupId(TLink link, LookupIdContext lookupIdContext);
        Tree<ReferenceToLoad> CreateReferenceTree(string linkTargetId, LoadLinkConfig config);
    }
}
