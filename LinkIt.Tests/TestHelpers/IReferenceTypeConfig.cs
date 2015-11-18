using System;
using LinkIt.PublicApi;

namespace LinkIt.Tests.TestHelpers
{
    public interface IReferenceTypeConfig
    {
        Type ReferenceType { get; }
        void Load(ILookupIdContext lookupIdContext, ILoadedReferenceContext loadedReferenceContext);
        string RequiredConnection { get; }
    }
}