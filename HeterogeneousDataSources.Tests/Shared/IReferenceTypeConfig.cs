using System;
using HeterogeneousDataSources.Protocols;

namespace HeterogeneousDataSources.Tests.Shared
{
    public interface IReferenceTypeConfig
    {
        Type ReferenceType { get; }
        void Load(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext);
        string RequiredConnection { get; }
    }
}