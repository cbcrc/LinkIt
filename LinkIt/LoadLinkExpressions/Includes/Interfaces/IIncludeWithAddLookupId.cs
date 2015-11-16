using System;
using LinkIt.Protocols;
using LinkIt.ReferenceTrees;

namespace LinkIt.LoadLinkExpressions.Includes.Interfaces {

    public interface IIncludeWithAddLookupId<TLink>:IInclude {
        Type ReferenceType { get; }
        void AddLookupId(TLink link, LookupIdContext lookupIdContext);
        void AddReferenceTree(string linkTargetId, ReferenceTree parent, LoadLinkProtocol loadLinkProtocol);
    }
}
