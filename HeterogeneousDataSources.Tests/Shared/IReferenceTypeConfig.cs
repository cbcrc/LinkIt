using System;

namespace HeterogeneousDataSources.Tests.Shared
{
    public interface IReferenceTypeConfig
    {
        //Key
        Type ReferenceType { get; }

        //stle: could be generalized
        void Load(LookupIdContext lookupIdContext, LoadedReferenceContext loadedReferenceContext);
        
        //cannot be generalized
        string RequiredConnection { get; }
    }
}