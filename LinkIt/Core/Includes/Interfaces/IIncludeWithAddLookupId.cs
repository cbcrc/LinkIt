using System;
using LinkIt.ReferenceTrees;

namespace LinkIt.Core.Includes.Interfaces {

    public interface IIncludeWithAddLookupId<TLink>:IInclude {
        Type ReferenceType { get; }
        void AddLookupId(TLink link, LookupIdContext lookupIdContext);
        void AddReferenceTree(string linkTargetId, ReferenceTree parent, LoadLinkProtocol loadLinkProtocol);
    }
}
