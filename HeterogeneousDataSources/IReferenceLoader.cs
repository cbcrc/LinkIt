using System;

namespace HeterogeneousDataSources
{
    public interface IReferenceLoader: IDisposable
    {
        void LoadReferences(
            LookupIdContext lookupIdContext, 
            LoadedReferenceContext loadedReferenceContext
        );
    }
}