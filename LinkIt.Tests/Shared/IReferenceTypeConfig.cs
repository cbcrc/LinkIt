using System;
using LinkIt.Protocols;

namespace LinkIt.Tests.Shared
{
    public interface IReferenceTypeConfig
    {
        Type ReferenceType { get; }
        void Load(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext);
        string RequiredConnection { get; }
    }
}