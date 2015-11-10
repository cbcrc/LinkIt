using System;

namespace HeterogeneousDataSources.Protocols
{
    public interface IReferenceLoader: IDisposable
    {
        void LoadReferences(
            LookupIdContext lookupIdContext, 
            LoadedReferenceContext loadedReferenceContext
        );
    }
}